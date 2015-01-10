using System;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    /// <summary>
    /// A class that can process different kinds of delegates and notifications
    /// </summary>
    public interface ITaskProcessor : IDisposable
    {
        /// <summary>
        /// Add a delegate handler to the store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        void AddHandler<T>(Action<T> target) where T : INotification;

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <param name="task"></param>
        void Process(DelegateTask task);

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        void Process<T>(DelegateTask<T> notification) where T : INotification;
    }
}
