using System;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    public interface ITaskProcessor : IDisposable
    {
        void AddHandler<T>(Action<T> target) where T : INotification;

        void Process(DelegateTask task);

        void Process<T>(DelegateTask<T> notification) where T : INotification;
    }
}
