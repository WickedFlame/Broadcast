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
        /// <typeparam name="T">Type of handler</typeparam>
        /// <param name="target">Task to process</param>
        void AddHandler<T>(Action<T> target) where T : INotification;

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <param name="task">The task to process</param>
        void Process(DelegateTask task);

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <typeparam name="T">Type of notification</typeparam>
        /// <param name="notification">The task to process</param>
        void Process<T>(DelegateTask<T> notification) where T : INotification;

        /// <summary>
        /// Processes a task without sending it to the notification handlers
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="task">The task</param>
        /// <returns>The result of the task</returns>
        T ProcessUnhandled<T>(DelegateTask<T> task);
    }
}
