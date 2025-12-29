namespace Broadcast
{
    /// <summary>
    /// 
    /// </summary>
    public class EventPublisher
    {
        private readonly HandlerSubscriptionCollection _subscriptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptions"></param>
        public EventPublisher(HandlerSubscriptionCollection subscriptions)
        {
            _subscriptions = subscriptions;
        }

        /// <summary>
        /// Publishes the specified event to all registered handlers for its type.
        /// </summary>
        /// <remarks>If no handlers are registered for the event's type, the method returns false and no
        /// action is taken. All handlers registered for the exact type of the event are invoked. Handlers for base
        /// types or interfaces are not invoked unless explicitly registered for the event's type.</remarks>
        /// <typeparam name="Tevent">The type of the event to publish.</typeparam>
        /// <param name="event">The event instance to publish to subscribed handlers. Cannot be null.</param>
        /// <returns>true if at least one handler was found and invoked for the event type; otherwise, false.</returns>
        public bool Publish<Tevent>(Tevent @event)
        {
            var key = @event.GetType();
            if (!_subscriptions.Any(h => h.EventType == key))
            {
                return false;
            }

            foreach (var registration in _subscriptions.Where(h => h.EventType == key))
            {
                var handler = registration.Handler as IEventHandler<Tevent>;
                if (handler == null)
                {
                    registration.TryHandle(@event);
                    continue;
                }

                handler.Handle(@event);
            }

            return true;
        }
    }
}
