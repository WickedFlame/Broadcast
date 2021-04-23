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
		/// Try to get all handlers for the type
		/// </summary>
		bool TryGetHandlers(Type key, out List<Action<INotification>> handlers);

		/// <summary>
		/// Add a notification handler to the store
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		void AddHandler<T>(Action<T> target) where T : INotification;
    }
}
