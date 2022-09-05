using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.EventSourcing
{
    public interface IEventSubscription
    {
        object Resolve();
    }

    public class TypeEventSubscription<THandler> : IEventSubscription
    {
        private readonly IActivationContext _activator;

        public TypeEventSubscription(IActivationContext activator)
        {
            SubscriberType = typeof(THandler);

            _activator = activator;
        }

        public Type SubscriberType { get; }

        public object Resolve()
        {
            return _activator.Resolve(SubscriberType);
        }
    }

    public class SingletonEventSubscription<TEvent> : IEventSubscription
    {
        private readonly Func<IEventHandler<TEvent>> _factory;

        public SingletonEventSubscription(Func<IEventHandler<TEvent>> factory)
        {
            _factory = factory;
        }

        public object Resolve()
        {
            return _factory.Invoke();
        }
    }
}
