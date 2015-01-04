using System;

namespace Broadcast.EventSourcing
{
    public interface ITaskProcessor : IDisposable
    {
        void Process(BackgroundTask job);
    }
}
