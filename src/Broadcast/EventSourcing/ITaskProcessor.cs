using System;

namespace Broadcast.EventSourcing
{
    public interface ITaskProcessor : IDisposable
    {
        void AddHandler<T>(Action<T> target) where T : INotification;

        void Process(BackgroundTask task);

        void Process<T>(NotificationTask<T> notification) where T : INotification;
    }
}
