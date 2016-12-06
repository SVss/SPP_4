using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    public class ExtThreadPool: IDisposable
    {
        private readonly ActionQueueAsync _queue;
        private readonly List<Thread> _workers;
        private bool _disposed = false;

        #region Public

        /// <summary>
        /// Create thread pool with <c>threadsCount</c> worker threads.
        /// </summary>
        /// <param name="threadsCount"></param>
        public ExtThreadPool(int threadsCount)
        {
            _queue = new ActionQueueAsync();
            _workers = new List<Thread>(threadsCount);

            for (var i = 0; i < threadsCount; ++i)
            {
                var worker = new Thread(WorkerBody);
                _workers.Add(worker);

                worker.Start(_queue);
            }
        }

        ~ExtThreadPool()
        {
            Dispose(false);
        }

        /// <summary>
        /// Add task to processing queue.
        /// </summary>
        /// <param name="task">Task to be enqueued.</param>
        /// <exception cref="ObjectDisposedException"></exception>
        public void EnqueueTask(Action task)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Can't enqueue task to disposed ExtThreadPool queue");
            }

            _queue.Enqueue(task);
        }

        /// <summary>
        /// Clear processing queue.
        /// Worker threads that already got tasks to process won't stop.
        /// </summary>
        public void ClearQueue()
        {
            _queue.Clear();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Internals

        private void Dispose(bool safe)
        {
            _queue.Clear();

            _disposed = true;
            _queue.Release();

            foreach (Thread worker in _workers)
            {
                worker.Join();
            }
        }

        private void WorkerBody(object actionQueue)
        {
            try
            {
                var queue = actionQueue as ActionQueueAsync;
                while (!_disposed)
                {
                    Action task = queue.Dequeue();
                    if (!_disposed)
                    {
                        task?.Invoke();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }
        #endregion
    }
}
