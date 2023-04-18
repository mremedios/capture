using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.TaskQueue
{
	using RxDisposable = System.Reactive.Disposables.Disposable;
	
	/// Осуществляет "накопление" переданных ему объектов и выполняет одну операцию для всей накопленной "пачки".
	public class BufferedTaskQueue<T> : IDisposable
	{
		private readonly Action<Exception, IList<T>> _exceptionHandler;
		private const int DefaultBufferSize = 1000;

		private const int MinBufferSize = 2;
		private const int MaxBufferSize = 100 * DefaultBufferSize;
		private readonly TimeSpan _defaultBufferTime = TimeSpan.FromSeconds(30);

		private readonly Lazy<(IObserver<T> observer, IDisposable disposableHandler)> _worker;
		
		public BufferedTaskQueue(
			Action<IList<T>> runner,
			Action<Exception, IList<T>> exceptionHandler = null,
			TimeSpan? bufferTimeSpan = null,
			int? bufferSize = null)
		{
			_exceptionHandler = exceptionHandler;
			_worker = new Lazy<(IObserver<T> observer, IDisposable disposableHandler)>(() => CreateObserver(
				bufferTimeSpan ?? _defaultBufferTime,
				bufferSize ?? DefaultBufferSize,
				buffer => buffer.Do(x =>
				{
					try
					{
						runner(x);
					}
					catch (Exception ex)
					{
						HandleException(ex, x);
					}
				})), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public BufferedTaskQueue(
			Func<IList<T>, Task> runner,
			Action<Exception, IList<T>> exceptionHandler = null,
			TimeSpan? bufferTimeSpan = null,
			int? bufferSize = null)
		{
			_exceptionHandler = exceptionHandler;
			_worker = new Lazy<(IObserver<T> observer, IDisposable disposableHandler)>(() =>  CreateObserver(
				bufferTimeSpan ?? _defaultBufferTime,
				bufferSize ?? DefaultBufferSize,
				buffer => buffer.SelectMany(async x =>
				{
					try
					{
						await runner(x).ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						HandleException(ex, x);
					}
					return Unit.Default;
				})
			), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public void EnqueueTask(T value)
		{
			_worker.Value.observer.OnNext(value);
		}

		public void EnqueueTasks(IEnumerable<T> values)
		{
			foreach (var value in values)
				_worker.Value.observer.OnNext(value);
		}

		private void HandleException(Exception ex, IList<T> data)
		{
			try
			{
				_exceptionHandler?.Invoke(ex, data);
			}
			catch (Exception ignored) {}
		}


		private static (IObserver<T> observer,IDisposable disposableHandler) CreateObserver<TResult>(
			TimeSpan bufferTimeSpan,
			int bufferSize,
			Func<IObservable<IList<T>>,IObservable<TResult>> runner
			)
		{
			var timeSpan = bufferTimeSpan;

			var subject = new Subject<T>();

			var buffer = subject
				.ObserveOn(TaskPoolScheduler.Default) // подписчик на события будет работать в другом потоке из пула
				.Buffer(timeSpan, bufferSize) // буферизация данных по времени и количеству событий
				.Where(x => !(x == null || x.Count == 0));

			var observerTask = runner(buffer)
				.LastOrDefaultAsync()
				.ToTask();

			var observer = subject;
			var disposableHandler = new CompositeDisposable {
				RxDisposable.Create(subject.OnCompleted),
				subject,
				RxDisposable.Create(() => observerTask.GetAwaiter().GetResult()),
				observerTask,
			};

			return (observer, disposableHandler);
		}

		public void Dispose()
		{
			if(_worker.IsValueCreated)
				_worker.Value.disposableHandler.Dispose();
		}
	}
}
