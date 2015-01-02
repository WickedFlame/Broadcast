using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Broadcast
{
    public class Mediator : IMediator
    {
        private readonly Dictionary<Type, List<Action<INotification>>> _handlers;

        public Mediator()
        {
            _handlers = new Dictionary<Type, List<Action<INotification>>>();
        }

        public void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification
        {
            RegisterHandler<T>(a => target.Handle(a));
        }

        public void RegisterHandler<T>(Action<T> target) where T : INotification
        {
            List<Action<INotification>> handlers;

            if (!_handlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<INotification>>();
                _handlers.Add(typeof(T), handlers);
            }

            handlers.Add(DelegateCaster.CastToBase<INotification, T>(x => target(x)));
        }

        public void Publish<T>(T notification) where T : INotification
        {
            List<Action<INotification>> handlers;

            if (!_handlers.TryGetValue(notification.GetType(), out handlers))
            {
                return;
            }

            foreach (var handler in handlers)
            {
                handler(notification);
            }
        }

        public async Task PublishAsync<T>(T notification) where T : INotification
        {
            List<Action<INotification>> handlers;

            if (!_handlers.TryGetValue(notification.GetType(), out handlers))
            {
                return;
            }

            foreach (var handler in handlers)
            {
                await Task.Run(() => handler(notification));
            }
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
