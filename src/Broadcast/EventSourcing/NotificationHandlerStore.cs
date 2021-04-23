using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Store for NotificationHandlers
	/// </summary>
    public class NotificationHandlerStore : INotificationHandlerStore
    {
        private readonly Dictionary<Type, List<Action<INotification>>> _handlers;

		/// <summary>
		/// Creates a new instance of the NotificationHandlerStore
		/// </summary>
        public NotificationHandlerStore()
        {
	        _handlers = new Dictionary<Type, List<Action<INotification>>>();
        }

		/// <summary>
		/// Try to get all handlers for the type
		/// </summary>
		/// <param name="key"></param>
		/// <param name="handlers"></param>
		/// <returns></returns>
        public bool TryGetHandlers(Type key, out List<Action<INotification>> handlers)
        {
	        return _handlers.TryGetValue(key, out handlers);
        }

        /// <summary>
        /// Add a notification handler to the store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void AddHandler<T>(Action<T> target) where T : INotification
        {
            List<Action<INotification>> handlers;

            if (!_handlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<INotification>>();
                _handlers.Add(typeof(T), handlers);
            }

            handlers.Add(DelegateCaster.CastToBase<INotification, T>(x => target(x)));
        }

        public class DelegateCaster
        {
            public static Action<TBase> CastToBase<TBase, TImpl>(Expression<Action<TImpl>> source) where TImpl : TBase
            {
                if (typeof(TImpl) == typeof(TBase))
                {
                    return (Action<TBase>)((Delegate)source.Compile());

                }

                var sourceParameter = Expression.Parameter(typeof(TBase), "source");
                var result = Expression.Lambda<Action<TBase>>(Expression.Invoke(source, Expression.Convert(sourceParameter, typeof(TImpl))), sourceParameter);
                return result.Compile();
            }
        }
    }
}
