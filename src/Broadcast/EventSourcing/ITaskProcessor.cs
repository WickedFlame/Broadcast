using System;

namespace Broadcast.EventSourcing
{
    public interface ITaskProcessor : IDisposable
    {
        void AddHandler<T>(Action<T> target) where T : INotification;

        void Process(DelegateTask task);

        void Process<T>(DelegateTask<T> notification) where T : INotification;
    }
}
