using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
    public interface ITaskStore : IEnumerable<WorkerTask>
    {
        IEnumerable<WorkerTask> CopyQueue();

        int CountQueue { get; }

        void Add(WorkerTask job);

        void SetInprocess(WorkerTask job);

        void SetProcessed(WorkerTask job);
    }
}
