using Broadcast.EventSourcing;
using Broadcast.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public class ProcessorContext : IProcessorContext
    {
        readonly INotificationHandlerStore _notificationHandlers;
		private ITaskProcessor _processor;

		public ProcessorContext()
            : this(new TaskStore())
        {
        }

        public ProcessorContext(ITaskStore store)
        {
            Store = store;
            _notificationHandlers = new NotificationHandlerStore();
        }

        /// <summary>
        /// Gets or sets the TaskSore containing all Tasks
        /// </summary>
        public ITaskStore Store { get; set; }

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
		        _processor = new TaskProcessor(Store, _notificationHandlers);
	        }

	        return _processor;
        }
    }
}
