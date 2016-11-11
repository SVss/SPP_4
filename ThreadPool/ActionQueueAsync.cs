using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// Asychronous <c>Queue</c> of <c>Actions</c>.
    /// </summary>
    class ActionQueueAsync
    {
        private readonly Queue<Action> _queue = new Queue<Action>();
        private readonly object _locker = new object();
        private bool _releasing = false;
        private int _readersCount = 0;

        // Public

        /// <summary>
        /// Enqueue new task.
        /// Waits until <c>ActionQueue</c> is unlocked.
        /// </summary>
        /// <param name="task">Task to be queued</param>
        public void Enqueue(Action task)
        {
            if (task == null)
                return;

            lock (_locker) { 
                _queue.Enqueue(task);
                Monitor.PulseAll(_locker);
            }
        }

        /// <summary>
        /// Dequeue task to be processed.
        /// Waits while <c>ActionQueue</c> is locked or empty.
        /// </summary>
        /// <returns>
        /// Action to be invoked
        /// or <c>null</c> if <c>Released</c> was called.
        /// </returns>
        public Action Dequeue()
        {
            Action result = null;
            lock (_locker)
            {
                while (_releasing)
                {
                    Monitor.Wait(_locker);
                }

                ++_readersCount;

                while (!_releasing &&(_queue.Count == 0))
                {
                    Monitor.Wait(_locker);
                }
                if (!_releasing)
                {
                    result = _queue.Dequeue();
                }

                --_readersCount;

                if (_releasing && (_readersCount == 0))
                {
                    _releasing = false;
                    Monitor.PulseAll(_locker);
                }
            }
            return result;
        }

        /// <summary>
        /// Release all threads waiting on <c>Dequeue</c> and give them <c>null</c> as <c>Dequeue</c> result.
        /// </summary>
        /// <remarks>
        /// All threads waiting on <c>Dequeue</c> call are released and get <c>null</c>.
        /// Other threads that enter <c>Dequeue</c> after <c>Release</c> call won't get <c>null</c>.
        /// </remarks>
        public void Release()
        {
            lock (_locker)
            {
                if (_readersCount > 0)
                {
                    _releasing = true;
                    Monitor.PulseAll(_locker);
                }
            }
        }

        /// <summary>
        /// Clear queue.
        /// </summary>
        public void Clear()
        {
            lock (_locker)
            {
                _queue.Clear();
            }
        }
    }
}
