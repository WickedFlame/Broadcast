﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
    public class NotificationHandlerStore : INotificationHandlerStore
    {
        Dictionary<Type, List<Action<INotification>>> _handlers;

        /// <summary>
        /// Sore of all INotification handlers
        /// </summary>
        public Dictionary<Type, List<Action<INotification>>> Handlers
        {
            get
            {
                if (_handlers == null)
                    _handlers = new Dictionary<Type, List<Action<INotification>>>();
                return _handlers;
            }
        }

        /// <summary>
        /// Add a notification handler to the store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void AddHandler<T>(Action<T> target) where T : INotification
        {
            List<Action<INotification>> handlers;

            if (!Handlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<INotification>>();
                Handlers.Add(typeof(T), handlers);
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
