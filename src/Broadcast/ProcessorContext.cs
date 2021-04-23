using Broadcast.EventSourcing;
using Broadcast.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using Broadcast.Configuration;
using Broadcast.Server;

namespace Broadcast
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public class ProcessorContext : IProcessorContext, IServerContext
    {
        readonly INotificationHandlerStore _notificationHandlers;
		private ITaskProcessor _processor;

		/// <summary>
		/// Creates a new instance of the ProcessorContext
		/// </summary>
		/// <param name="store"></param>
        public ProcessorContext(ITaskStore store)
        {
            Store = store;
            _notificationHandlers = new NotificationHandlerStore();
        }

        /// <summary>
        /// Gets or sets the TaskSore containing all Tasks
        /// </summary>
        public ITaskStore Store { get; set; }

        public Options Options { get; set; }

		/// <summary>
		/// Gets the TaskQueue
		/// </summary>
		public ITaskQueue Queue => Open().Queue;

        /// <summary>
        /// Gets all Tasks that have been processed
        /// </summary>
        public IEnumerable<ITask> ProcessedTasks
        {
            get
            {
                return Store.Where(s => s.State == TaskState.Processed);
            }
        }

        /// <summary>
        /// Gets the store of the NotificationHandlers
        /// </summary>
        public INotificationHandlerStore NotificationHandlers => _notificationHandlers;

        /// <summary>
        /// Creates a new TaskProcessor that can be used to process the given task
        /// </summary>
        /// <returns></returns>
        public ITaskProcessor Open()
        {
	        if (_processor == null)
	        {
		        _processor = new TaskProcessor(this);
	        }

	        return _processor;
        }
    }
}
