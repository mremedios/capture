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
        private readonly ActionBlock<T> _tasks;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Action<Exception, T> _exceptionHandler;
        private Action<int> _checkForThresholds;

        private static int DefaultMaxDegreeOfParallelism => Environment.ProcessorCount * 2;
        
        public TaskQueue(Func<T, Task> runner, Action<Exception, T> exceptionHandler = null, int? workerCount = null)
        {
            _exceptionHandler = exceptionHandler;
            _tasks = new ActionBlock<T>(
                async x =>
                {
                    try
                    {
                        await runner(x).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, x);
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    CancellationToken = _cts.Token,
                    MaxDegreeOfParallelism = workerCount ?? DefaultMaxDegreeOfParallelism
                });
        }
        
        public TaskQueue(Action<T> runner, Action<Exception, T> exceptionHandler = null, int? workerCount = null)
        {
            Action<T> safeRunner = (args) =>
            {
                try
                {
                    runner(args);
                }
                catch (Exception ex)
                {
                    HandleException(ex, args);
                }
            };

            _exceptionHandler = exceptionHandler;
            _tasks = new ActionBlock<T>(safeRunner,
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
            var result = _tasks.Post(value);

            return result;
        }

        public int TaskCount => _tasks.InputCount;

        private void HandleException(Exception ex, T data)
        {
            try
            {
                _exceptionHandler?.Invoke(ex, data);
            }
            catch (Exception ignored)
            {
                
            }
        }
    }
}