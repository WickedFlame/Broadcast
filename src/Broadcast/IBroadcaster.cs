using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Broadcast
{
    public interface IBroadcaster
    {
        /// <summary>
        /// Gets the ProcessorContext that containes all information create a TaskProcessor
        /// </summary>
        IProcessorContext Context { get; }

        /// <summary>
        /// Send a delegate to the task processor
        /// </summary>
        /// <param name="task"></param>
        void Send(Expression<Action> task);

        //void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback);
        
        /// <summary>
        /// Sends a INotification to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        void Send<T>(Expression<Func<T>> notification) where T : INotification;

        /// <summary>
        /// Sends a INotification async to the processor. The INotification will be passed to all registered Handlers of the same type. Async method calls are only allowed in ProcessorMode.Default
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <returns></returns>
        Task SendAsync<T>(Expression<Func<T>> notification) where T : INotification;

        /// <summary>
        /// Register a INotificationTarget that gets called when a INotification of the same type is sent
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="target">The INotificationTarget that handles the INotification</param>
        void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification;

        /// <summary>
        /// Register a delegate that gets called when a INotification of the same type is sent
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="target">The delegate that handles the INotification</param>
        void RegisterHandler<T>(Action<T> target) where T : INotification;
    }
}
