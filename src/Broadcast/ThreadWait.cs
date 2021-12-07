using System;
using System.Threading.Tasks;

namespace Broadcast
{
    /// <summary>
    /// Is used to regulate threads with recurring loop
    /// </summary>
    public class ThreadWait : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public ThreadWait()
        {
            IsOpen = true;
        }

        /// <summary>
        /// Gets the state of the threadwait
        /// </summary>
        public bool IsOpen{ get; private set; }

        /// <summary>
        /// Wait on the tread for the given amount of milliseconds
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public async Task WaitOne(int delay)
        {
            await Task.Delay(delay).ConfigureAwait(false);
        }

        /// <summary>
        /// Close the threadwait
        /// </summary>
        public void Close()
        {
            IsOpen = false;
        }

        /// <summary>
        /// Close and dispose the TaskWait
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Close and dispose the TaskWait
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            Close();
        }
    }
}
