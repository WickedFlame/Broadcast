using Broadcast.EventSourcing;
using System;
using System.Collections.Generic;

namespace Broadcast
{
    public interface IProcessorContext
    {
        ITaskProcessor Open();

        ITaskStore Store { get; set; }

        IEnumerable<WorkerTask> ProcessedTasks { get; }

        ProcessorMode Mode { get; set; }
    }
}
