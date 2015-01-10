using System;
using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// A store containing all registered INotification delegate handlers
    /// </summary>
    public interface INotificationHandlerStore
    {
        /// <summary>
        /// Sore of all INotification handlers
        /// </summary>
        Dictionary<Type, List<Action<INotification>>> Handlers { get; }

        /// <summary>
        /// Add a notification handler to the store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        void AddHandler<T>(Action<T> target) where T : INotification;
    }
}
