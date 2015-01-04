using Broadcast.EventSourcing;
using System.Collections.Generic;

namespace Broadcast
{
    public interface IProcessorContext
    {
        ITaskProcessor Open();

        ITaskStore TaskStore { get; set; }

        IEnumerable<BackgroundTask> ProcessedTasks { get; }

        ProcessorMode Mode { get; set; }
    }
}
