using Broadcast.EventSourcing;
using Broadcast.Processing;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets all Tasks that have been processed
        /// </summary>
        IEnumerable<ITask> ProcessedTasks { get; }

        /// <summary>
        /// Gets the store of the NotificationHandlers
        /// </summary>
        INotificationHandlerStore NotificationHandlers { get; }

        /// <summary>
        /// Gets or sets the ProcessorMode the Processor runs in
        /// </summary>
        ProcessorMode Mode { get; set; }

        /// <summary>
        /// Creates a new TaskProcessor that can be used to process the given task
        /// </summary>
        /// <returns></returns>
        ITaskProcessor Open();
    }
}
