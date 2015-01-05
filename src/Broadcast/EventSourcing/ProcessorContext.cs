using System.Linq;
using Broadcast.EventSourcing;
using System;
using System.Collections.Generic;

namespace Broadcast
{
    public class ProcessorContext : IProcessorContext
    {
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

        ITaskStore _store;
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

        public IEnumerable<WorkerTask> ProcessedTasks
        {
            get
            {
                return _store.Where(s => s.State == TaskState.Processed);
            }
        }

        readonly INotificationHandlerStore _notificationHandlers;
        internal INotificationHandlerStore NotificationHandlers
        {
            get
            {
                return _notificationHandlers;
            }
        }

        public ProcessorMode Mode { get; set; }

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
