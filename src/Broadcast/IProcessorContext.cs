using Broadcast.EventSourcing;
using Broadcast.Processing;
using System.Collections.Generic;
using Broadcast.Configuration;

namespace Broadcast
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public interface IProcessorContext
    {
        /// <summary>
        /// Gets or sets the TaskSore containing all Tasks
        /// </summary>
        ITaskStore Store { get; set; }

        Options Options { get; set; }

		/// <summary>
		/// Gets the TaskQueue
		/// </summary>
		ITaskQueue Queue { get; }

        /// <summary>
        /// Gets all Tasks that have been processed
        /// </summary>
        IEnumerable<ITask> ProcessedTasks { get; }

        /// <summary>
        /// Gets the store of the NotificationHandlers
        /// </summary>
        INotificationHandlerStore NotificationHandlers { get; }
		
        /// <summary>
        /// Creates a new TaskProcessor that can be used to process the given task
        /// </summary>
        /// <returns></returns>
        ITaskProcessor Open();
    }
}
