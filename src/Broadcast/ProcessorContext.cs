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
        ITaskStore _store;

        public ProcessorContext()
            : this(TaskStoreFactory.GetStore(), ProcessorContextFactory.GetMode())
        {
        }

        public ProcessorContext(ProcessorMode mode)
            : this(TaskStoreFactory.GetStore(), mode)
        {
        }

        public ProcessorContext(ITaskStore store)
            : this(store, ProcessorContextFactory.GetMode())
        {
        }

        public ProcessorContext(ITaskStore store, ProcessorMode mode)
        {
            _store = store;
            Mode = mode;
            _notificationHandlers = new NotificationHandlerStore();
        }

        /// <summary>
        /// Gets or sets the TaskSore containing all Tasks
        /// </summary>
        public ITaskStore Store
        {
            get
            {
                return _store;
            }
            set
            {
                _store = value;
            }
        }

        /// <summary>
        /// Gets all Tasks that have been processed
        /// </summary>
        public IEnumerable<WorkerTask> ProcessedTasks
        {
            get
            {
                return _store.Where(s => s.State == TaskState.Processed);
            }
        }

        /// <summary>
        /// Gets the sore of the NotificationHandlers
        /// </summary>
        internal INotificationHandlerStore NotificationHandlers
        {
            get
            {
                return _notificationHandlers;
            }
        }

        /// <summary>
        /// Gets or sets the ProcessorMode the Processor runs in
        /// </summary>
        public ProcessorMode Mode { get; set; }

        /// <summary>
        /// Creates a new TaskProcessor that can be used to process the given task
        /// </summary>
        /// <returns></returns>
        public ITaskProcessor Open()
        {
            switch (Mode)
            {
                case ProcessorMode.Default:
                    return new TaskProcessor(_store, _notificationHandlers);

                case ProcessorMode.Background:
                    return new BackgroundTaskProcessor(_store, _notificationHandlers);

                case ProcessorMode.Async:
                    return new AsyncTaskProcessor(_store, _notificationHandlers);

                default:
                    throw new InvalidOperationException(string.Format("The specified Processor mode {0} is not supported", Mode));
            }
        }
    }
}
