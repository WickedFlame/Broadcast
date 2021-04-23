using System;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    /// <summary>
    /// A class that can process different kinds of delegates and notifications
    /// </summary>
    public interface ITaskProcessor : IDisposable
    {
		/// <summary>
		/// Gets the TaskQueue
		/// </summary>
		ITaskQueue Queue { get; }

		/// <summary>
		/// Wait for all threads to end
		/// </summary>
        void WaitAll();

		/// <summary>
		/// Process the delegate task
		/// </summary>
		/// <param name="task">The task to process</param>
        void Process(ITask task);
    }
}
