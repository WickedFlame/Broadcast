using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Represents a store conatining all Tasks
    /// </summary>
    public interface ITaskStore : IEnumerable<ITask>
    {
        /// <summary>
        /// Adds a new Task to the queue to be processed
        /// </summary>
        /// <param name="task"></param>
        void Add(ITask task);

		/// <summary>
		/// Sets tha task to InProcess mode
		/// </summary>
		/// <param name="task"></param>
		void SetInprocess(ITask task);

        /// <summary>
        /// Sets the task to Processed mode and removes it from the process queue
        /// </summary>
        /// <param name="task"></param>
        void SetProcessed(ITask task);
    }
}
