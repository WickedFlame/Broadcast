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
            Store = store;
            Mode = mode;
            _notificationHandlers = new NotificationHandlerStore();
        }

        /// <summary>
        /// Gets or sets the TaskSore containing all Tasks
        /// </summary>
        public ITaskStore Store { get; set; }

        /// <summary>
        /// Gets all Tasks that have been processed
        /// </summary>
        public IEnumerable<BroadcastTask> ProcessedTasks
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
                case ProcessorMode.Parallel:
                    return new TaskProcessor(Store, _notificationHandlers);

                case ProcessorMode.Background:
                    return new BackgroundTaskProcessor(Store, _notificationHandlers);

                case ProcessorMode.Async:
                    return new AsyncTaskProcessor(Store, _notificationHandlers);

                default:
                    throw new InvalidOperationException($"The specified Processor mode {Mode} is not supported");
            }
        }
    }
}
