using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.TaskQueue;

public class CustomBufferedQueue<T> : IDisposable
{
	private const int BufferSize = 1000;
	private const int MaxBufferSize = BufferSize * 2;

	private bool _isDisposed;
	private SemaphoreSlim _locker = new SemaphoreSlim(1, 1);

	private readonly ConcurrentStack<List<T>> _listBuffer;
	private int _size;
	private List<T> _list;
	private readonly TaskQueue<List<T>> _queue;
	private readonly Func<List<T>, Task> _action;
	private readonly Timer _timer;

	private readonly ILogger<CustomBufferedQueue<T>> _logger;

	public CustomBufferedQueue(Func<List<T>, Task> action, ILogger<CustomBufferedQueue<T>> logger)
	{
		_logger = logger;
		_action = action;
		_listBuffer = new ConcurrentStack<List<T>>();
		_queue = new TaskQueue<List<T>>(HandleBatch, SavingErrorHandler, 1);
		_timer = new Timer(_ => RunFlush());

		for (int i = 0; i < 10; i++)
		{
			_listBuffer.Push(new List<T>(MaxBufferSize));
		}

		Flush();

		_timer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
	}

	public void Dispose()
	{
		_isDisposed = true;
		_locker?.Dispose();
		_timer?.Dispose();

	}

	public void Push(T item)
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException(nameof(CustomBufferedQueue<T>));
		}

		_list.Add(item);
		Interlocked.Increment(ref _size);

		if (_size > BufferSize)
		{
			RunFlush();
		}
	}

	private async Task HandleBatch(List<T> list)
	{
		//Дожидаемся добавления в список сообщений, которые
		//обработались после Interlocked.Exchange
		await Task.Delay(50).ConfigureAwait(false);

		try
		{
			if (list.Count > 0)
			{
				await _action(list).ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "HandleBatch error");
		}
		finally
		{
			list.Clear();
			_listBuffer.Push(list);
		}
	}

	private void RunFlush()
	{
		Task.Run(Flush).ContinueWith(t => t.LogTaskFailState(_logger));
	}

	private void Flush()
	{
		if (_locker.Wait(0))
		{
			try
			{
				if (!_listBuffer.TryPop(out var newlist))
				{
					_logger.LogError("List buffer is empty");
					return;
				}

				var oldList = Interlocked.Exchange(ref _list, newlist);
				Interlocked.Exchange(ref _size, _list!.Count);
				if (oldList != null)
				{
					_queue.EnqueueTask(oldList);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Flush error");
			}
			finally
			{
				_locker.Release();
				_timer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
			}
		}
	}



	private void SavingErrorHandler(Exception e, IList<T> list)
	{
		_logger.LogWarning("Error saving message: {0}", e.Message);
	}

	/*private class ListWrapper : IList<T>, IDisposable
	{
	   private readonly List<T> _list;
	   private readonly ConcurrentStack<List<T>> _container;

	   public ListWrapper(List<T> list, ConcurrentStack<List<T>> container)
	   {
		  _list = list;
		  _container = container;
	   }


	   public void Dispose()
	   {
		  _list.Clear();
		  _container.Push(_list);
		  _list = null;
	   }

	   public IEnumerator<T> GetEnumerator()
	   {
		  throw new NotImplementedException();
	   }

	   IEnumerator IEnumerable.GetEnumerator()
	   {
		  return GetEnumerator();
	   }

	   public void Add(T item)
	   {
		  _list.Add(item);
	   }

	   public void Clear()
	   {
		  _list.Clear();
	   }

	   public bool Contains(T item)
	   {
		  throw new NotImplementedException();
	   }

	   public void CopyTo(T[] array, int arrayIndex)
	   {
		  throw new NotImplementedException();
	   }

	   public bool Remove(T item)
	   {
		  throw new NotImplementedException();
	   }

	   public int Count => _list.Count;
	   public bool IsReadOnly => false;
	   public int IndexOf(T item)
	   {
		  return _list.IndexOf(item);
	   }

	   public void Insert(int index, T item)
	   {
		  throw new NotImplementedException();
	   }

	   public void RemoveAt(int index)
	   {
		  throw new NotImplementedException();
	   }

	   public T this[int index]
	   {
		  get => _list[index];

		  set => _list[index] = value;
	   }
	}*/
}


internal static class Helper
{

	public static void LogTaskFailState<T>(this Task task, ILogger<CustomBufferedQueue<T>> logger = null)
	{
		if (task.IsFaulted)
		{
			logger?.LogError(task.Exception, nameof(LogTaskFailState));
		}

		if (task.IsCanceled)
		{
			logger?.LogWarning("TaskWasCanceled");
		}
	}
}