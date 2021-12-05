using System;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Processing
{
    /// <summary>
    /// Counts all threads that are allocated by the <see cref="IThreadList"/>
    /// </summary>
    public class ThreadCounter : IDisposable
    {
        private static readonly Dictionary<string, int> _counters = new Dictionary<string, int>();

        /// <summary>
        /// Creates a new ThreadCounter and registers all events to <see cref="IThreadList"/>
        /// </summary>
        /// <param name="threads"></param>
        public ThreadCounter(IThreadList threads)
        {
            threads.ThreadCountHandler += Threads_ThreadCountHandler;
        }

        private void Threads_ThreadCountHandler(object sender, ThreadHandlerEventArgs e)
        {
            lock (_counters)
            {
                _counters[e.Name] = e.Count;
            }
        }

        /// <summary>
        /// Dispose the ThreadCounter
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Get the sum over all Threads registered to <see cref="IThreadList"/>
        /// </summary>
        /// <returns></returns>
        public static int GetTotalThreadCount()
        {
            lock (_counters)
            {
                return _counters.Values.Sum();
            }
        }
    }
}
