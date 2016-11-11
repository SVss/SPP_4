using System;
using System.Collections.Generic;
using System.Threading;

namespace RssReader.Utils
{
    class ThreadPool: IDisposable
    {
        
        // Public

        public ThreadPool(int threadsCount)
        {
            throw new NotImplementedException();
        }

        ~ThreadPool()
        {
            Dispose(false);
        }

        public void QueueTask(Action task)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Internals

        private void Dispose(bool safe)
        {
            throw new NotImplementedException();
        }

        private void WorkerBody(object actionQueue)
        {
            throw new NotImplementedException();
        }

    }
}
