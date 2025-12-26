using System.Reflection;

namespace Broadcast
{
    public class EventBus : IEventBus
    {
        private readonly List<MessageHandlerRegistration> _handlers = [];
        private readonly IEventStore _eventStore;

        public EventBus() 
            : this(new InMemoryEventStore())
        {
        }

        public EventBus(IEventStore eventStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException("eventStore");
        }

        public void Subscribe<Tevent>(IMessageHandler<Tevent> handler)
        {
            _handlers.Add(new MessageHandlerRegistration(typeof(Tevent), handler));
        }

        /// <summary>
        /// Send the <see cref="IEvent"/> to the <see cref="IMessageHandler{T}"/> without publishing to the EventStore.
        /// This is used when a Event has to be processed but not habe the ability to be recreated from the EventStore.
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="event"></param>
        public virtual void Send<Tevent>(Tevent @event)
        {
            var key = @event.GetType();
            if (!_handlers.Any(h => h.EventType == key))
            {
                Console.WriteLine($"No handler for event type {key}");
                return;
            }

            foreach (var registration in _handlers.Where(h => h.EventType == key))
            {
                var handler = registration.Handler as IMessageHandler<Tevent>;
                if (handler == null)
                {
                    registration.TryHandle(@event);
                    return;
                }

                handler.Handle(@event);
            }
        }

        /// <summary>
        /// Publish the <see cref="IEvent"/> to the EventStore and send the <see cref="IEvent"/> to the <see cref="IMessageHandler{T}"/>
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="event"></param>
        public void Publish<Tevent>(string id, DateTime time, Tevent @event) where Tevent : IEvent
        {
            _eventStore.Add(id, time, @event);
            Send(@event);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose here
            }
        }
    }
}
