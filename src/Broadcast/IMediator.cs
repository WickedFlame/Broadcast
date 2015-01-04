using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Broadcast
{
    public interface IMediator
    {
        /// <summary>
        /// Register a <see cref="INotificationTarget"/> that gets called when the specific <see cref="INotification"/> is published
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification;

        /// <summary>
        /// Register a delegate that gets called when the specific <see cref="INotification"/> is published
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        void RegisterHandler<T>(Action<T> target) where T : INotification;

        /// <summary>
        /// Publishes a <see cref="INotification"/> and passes it to the registered NotificationTargets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        void Send<T>(Expression<Func<T>> notification) where T : INotification;

        /// <summary>
        /// Publishes a <see cref="INotification"/> and passes it to the registered NotificationTargets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendAsync<T>(Expression<Func<T>> notification) where T : INotification;
    }
}
