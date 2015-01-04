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
        }

        ITaskStore _store;
        public ITaskStore TaskStore
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

        public IEnumerable<BackgroundTask> ProcessedTasks
        {
            get
            {
                return _store;
            }
        }

        public ProcessorMode Mode { get; set; }

        public ITaskProcessor Open()
        {
            switch (Mode)
            {
                case ProcessorMode.Default:
                    return new TaskProcessor(_store);

                case ProcessorMode.Background:
                    return new BackgroundTaskProcessor(_store);

                case ProcessorMode.Async:
                    return new AsyncTaskProcessor(_store);

                default:
                    throw new InvalidOperationException(string.Format("The specified Processor mode {0} is not supported", Mode));
            }
        }
    }
}
