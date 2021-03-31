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
		/// Set the state of the task
		/// </summary>
		/// <param name="task"></param>
		/// <param name="state"></param>
        void SetState(ITask task, TaskState state);
    }
}
