using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Capture.Service.TaskQueue
{
	public class TaskQueue<T> : IDisposable
	{
		private readonly ActionBlock<(T Message, EnqueueContext Context)> _tasks;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly Action<Exception, T> _exceptionHandler;
		private Action<int> _checkForThresholds;

		private static int DefaultMaxDegreeOfParallelism => Environment.ProcessorCount * 2;

		/// <summary>
		/// Инициализирует новый экземпляр класса.
		/// </summary>
		/// <param name="runner">метод, который обрабатывает задание</param>
		/// <param name="exceptionHandler">обработчик исключений, может быть null</param>
		/// <param name="workerCount">Количество потоков, обрабатывающих задания</param>
		public TaskQueue(Func<T, Task> runner, Action<Exception, T> exceptionHandler = null, int? workerCount = null)
		{
			_exceptionHandler = exceptionHandler;
			_tasks = new ActionBlock<(T Message, EnqueueContext Context)>(
				async x =>
				{
					using (LogicalContext.Set(x.Context.Items))
					{

						try
						{
							await runner(x.Message).ConfigureAwait(false);
						}
						catch (Exception ex)
						{
							HandleException(ex, x.Message);
						}
					}
				},
				new ExecutionDataflowBlockOptions
				{
					CancellationToken = _cts.Token,
					MaxDegreeOfParallelism = workerCount ?? DefaultMaxDegreeOfParallelism
				});
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса.
		/// </summary>
		/// <param name="runner">метод, который обрабатывает задание</param>
		/// <param name="exceptionHandler">обработчик исключений, может быть null</param>
		/// <param name="workerCount">Количество потоков, обрабатывающих задания</param>
		public TaskQueue(Action<T> runner, Action<Exception, T> exceptionHandler = null, int? workerCount = null)
		{
			Action<(T Message, EnqueueContext Context)> safeRunner = (args) =>
			{
				using (LogicalContext.Set(args.Context.Items))
				{
					try
					{
						runner(args.Message);
					}
					catch (Exception ex)
					{
						HandleException(ex, args.Message);
					}
				}
			};

			_exceptionHandler = exceptionHandler;
			_tasks = new ActionBlock<(T Message, EnqueueContext Context)>(safeRunner,
				new ExecutionDataflowBlockOptions
				{
					CancellationToken = _cts.Token,
					MaxDegreeOfParallelism = workerCount ?? DefaultMaxDegreeOfParallelism
				});
		}
		

		private int _countMessages;
		
		public void Dispose()
		{
			Stop();
			_cts.Dispose();
		}
		public bool Stop(bool wait = true, TimeSpan? timeout = null)
		{
			var stopped = true;
			_tasks.Complete();
			try
			{
				if (!_cts.IsCancellationRequested)
				{
					if (wait)
					{
						if (timeout.HasValue)
						{
							stopped = _tasks.Completion.Wait(timeout.Value);
						}
						else
						{
							_tasks.Completion.Wait();
						}
					}

					try
					{
						_cts.Cancel();
					}
					catch (OperationCanceledException)
					{
					}
					catch (AggregateException ae) when (ae.InnerExceptions.All(x => x is OperationCanceledException))
					{
						HandleException(ae, default);
					}
				}
			}
			catch (Exception ex)
			{
				HandleException(ex, default);
			}

			return stopped;
		}

		public bool EnqueueTask(T value)
		{
			var result = _tasks.Post((value, new EnqueueContext(LogicalContext.GetAll())));
			_checkForThresholds?.Invoke(1);

			return result;
		}

		public void EnqueueTasks(IEnumerable<T> values)
		{
			int addedCount = 0;
			var context = new EnqueueContext(LogicalContext.GetAll());
			foreach (var value in values)
			{
				_tasks.Post((value, context));
				addedCount++;
			}

			_checkForThresholds?.Invoke(addedCount);
		}
		
		public int TaskCount => _tasks.InputCount;

		private void HandleException(Exception ex, T data)
		{
			try
			{
				_exceptionHandler?.Invoke(ex, data);
			}
			catch (Exception exception)
			{
				// Logger.Error(new AggregateException(ex, exception));
			}
		}

		private class EnqueueContext
		{
			public EnqueueContext((string Key, string Value)[] items)
			{
				Items = items;
			}

			public (string Key, string Value)[] Items { get; }
		}
	}
}
