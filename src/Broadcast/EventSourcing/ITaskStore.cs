using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
    public interface ITaskStore : IEnumerable<BackgroundTask>
    {
        IEnumerable<BackgroundTask> CopyQueue();

        int CountQueue { get; }

        void Add(BackgroundTask job);

        void SetInprocess(BackgroundTask job);

        void SetProcessed(BackgroundTask job);
    }
}
