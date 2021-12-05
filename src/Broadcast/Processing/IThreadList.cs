using System;
using System.Threading.Tasks;

namespace Broadcast.Processing
{
    /// <summary>
    /// A list of running threads
    /// </summary>
    public interface IThreadList
    {
        /// <summary>
        /// The Eventhandler used to notify when a thread is created or removed
        /// </summary>
        event EventHandler<ThreadHandlerEventArgs> ThreadCountHandler;

        /// <summary>
        /// Add a task to the threadlist
        /// </summary>
        /// <param name="task"></param>
        void Add(Task task);

        /// <summary>
        /// Remove a task from the threadlist
        /// </summary>
        /// <param name="task"></param>
        void Remove(Task task);

        /// <summary>
        /// Wait for all threads to end
        /// </summary>
        void WaitAll();

        /// <summary>
        /// Gets the count of threads in the list
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}
