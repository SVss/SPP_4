using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    public class ExtThreadPool: IDisposable
    {
        private readonly Queue<Action> _queue;
        private readonly List<Thread> _threads;

        private readonly object _sync = new object();

        private const int StdTimeoutMs = 50;

        // Public

        public ExtThreadPool(int threadsCount)
        {
            _queue = new Queue<Action>();
            _threads = new List<Thread>();

            AddThreads(threadsCount);
        }

        ~ExtThreadPool()
        {
            Dispose(false);
        }

        public void EnqueueTask(Action task)
        {
            if (task == null)
                return;

            lock (_sync)
            {
                _queue.Enqueue(task);
                Monitor.Pulse(_sync);
            }
        }

        public void Reinit(int threadsCount)
        {
            if (threadsCount > _threads.Count)
            {
                AddThreads(threadsCount - _threads.Count);
            }
            else if (threadsCount < _threads.Count)
            {
                StopThreads();
                AddThreads(threadsCount);
            }
        }

        public void ClearQueue()
        {
            lock (_queue)
            {
                _queue.Clear();
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
            StopThreads();
        }

        private void StopThreads()
        {
            lock (_sync)
            {
                foreach (Thread thread in _threads)
                {
                    _queue.Enqueue(null);
                    Monitor.Pulse(_sync);
                }
            }

            foreach (Thread thread in _threads)
            {
                thread.Join(StdTimeoutMs);
            }

            _threads.Clear();
        }

        private void AddThreads(int n)
        {
            for (int i = 0; i < n; ++i)
            {
                var thread = new Thread(WorkerCycle);
                thread.IsBackground = true;

                _threads.Add(thread);
                thread.Start();
            }
        }
        
        private Action DequeueTask()
        {
            lock (_sync)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_sync);
                }
                Action result = _queue.Dequeue();
                return result;
            }
        }

        private void WorkerCycle()
        {
            while (true)
            {
                var task = DequeueTask();
                if (task == null)
                    break;

                task.Invoke();
            }
        }
    }
}
