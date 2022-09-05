using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
    public interface IEventHandlerContext
    {
        IActivationContext ActivationContext { get; }

        /// <summary>
        /// Subscribe to the Event of type TEvent
        /// </summary>
        /// <param name="subscription"></param>
        /// <typeparam name="TEvent"></typeparam>
        IEventHandlerContext Register<TEvent>(IEventSubscription subscription);

        IEnumerable<IEventHandler<T>> Resolve<T>();
    }

    public class EventHandlerContext : IEventHandlerContext
    {
        private readonly Dictionary<Type, List<IEventSubscription>> _subscriptions = new Dictionary<Type, List<IEventSubscription>>();

        public EventHandlerContext(IActivationContext activator)
        {
            ActivationContext = activator;
        }

        public IActivationContext ActivationContext { get; }

        /// <summary>
        /// Subscribe to the Event of type TEvent
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        public IEventHandlerContext Register<TEvent>(IEventSubscription subscription)
        {
            var eventType = typeof(TEvent);
            if (!_subscriptions.ContainsKey(eventType))
            {
                _subscriptions.Add(eventType, new List<IEventSubscription>());
            }

            _subscriptions[eventType].Add(subscription);

            return this;
        }

        public IEnumerable<IEventHandler<T>> Resolve<T>()
        {
            if (!_subscriptions.ContainsKey(typeof(T)))
            {
                return Enumerable.Empty<IEventHandler<T>>();
            }

            var resolved = new List<IEventHandler<T>>();

            var subscriptions = _subscriptions[typeof(T)];
            foreach (var subscription in subscriptions)
            {
                var handler = subscription.Resolve() as IEventHandler<T>;
                if (handler == null)
                {
                    continue;
                }

                resolved.Add(handler);
            }
            return resolved;
        }
    }

    public static class MessageBrokerExtensions
    {
        /// <summary>
        /// Subscribe to the Event of type TEvent
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        public static IEventHandlerContext Register<TEvent, THandler>(this IEventHandlerContext broker) where THandler : IEventHandler<TEvent>
        {
            broker.Register<TEvent>(new TypeEventSubscription<THandler>(broker.ActivationContext));

            return broker;
        }
    }
}
