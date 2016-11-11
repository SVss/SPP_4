using System;
using System.Collections.Generic;
using System.Threading;

namespace RssReader.Utils
{
    /// <summary>
    /// Asychronous <c>Queue</c> of <c>Actions</c>.
    /// </summary>
    class ActionQueueAsync
    {
        // Public

        /// <summary>
        /// Enqueue new task.
        /// Waits until <c>ActionQueue</c> is unlocked.
        /// </summary>
        /// <param name="task">Task to be queued</param>
        public void Enqueue(Action task)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get task from queue's peek.
        /// Waits while <c>ActionQueue</c> is locked or empty.
        /// </summary>
        /// <returns>
        /// Action to be invoked
        /// or <c>null</c> if <c>Released</c> was called.
        /// </returns>
        public Action Dequeue()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear queue.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
