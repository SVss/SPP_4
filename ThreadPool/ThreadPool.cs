using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    public class ThreadPool: IDisposable
    {
        private readonly ActionQueueAsync _queue;
        private readonly List<Thread> _workers;
        private bool _disposed = false;

        // Public

        public ThreadPool(int threadsCount)
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

        ~ThreadPool()
        {
            Dispose(false);
        }

        public void QueueTask(Action task)
        {
            if (!_disposed)
            {
                _queue.Enqueue(task);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Internals

        private void Dispose(bool safe)
        {
            _disposed = true;
            _queue.Release();

            foreach (var worker in _workers)
            {
                worker.Join();
            }
            _queue.Clear();
        }

        private void WorkerBody(object actionQueue)
        {
            while (!_disposed)
            {
                Action task = (actionQueue as ActionQueueAsync)?.Dequeue();
                if (!_disposed)
                {
                    task?.Invoke();
                }
            }
        }

    }
}
