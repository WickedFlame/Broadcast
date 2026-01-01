using System.Reflection;

namespace Broadcast
{
    /// <summary>
    /// 
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly HandlerSubscriptionCollection _handlers = [];
        private readonly EventStoreOptions _options;
        private readonly IEventStore _eventStore;
        private readonly EventPublisher _publisher;

        /// <summary>
        /// 
        /// </summary>
        public EventBus() 
            : this(new InMemoryEventStore())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventStore"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EventBus(IEventStore eventStore)
            : this(eventStore, new EventStoreOptions())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventStore"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EventBus(IEventStore eventStore, EventStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException("options");
            _eventStore = eventStore ?? throw new ArgumentNullException("eventStore");
            _publisher = new EventPublisher(_handlers);
        }

        /// <summary>
        /// Gets a enumeration of all registered message handlers.
        /// </summary>
        public IEnumerable<IEventHandler> Handlers => _handlers.Select(h => h.Handler);

        /// <summary>
        /// Registers the specified message handler to receive events of the given type.
        /// </summary>
        /// <remarks>Multiple handlers can be registered for the same event type. Handlers will be invoked
        /// when events of the subscribed type are published.</remarks>
        /// <typeparam name="Tevent">The type of event messages to subscribe to.</typeparam>
        /// <param name="handler">The message handler that will process incoming events of type <typeparamref name="Tevent"/>. Cannot be null.</param>
        public void Subscribe<Tevent>(IEventHandler<Tevent> handler)
        {
            _handlers.Add(typeof(Tevent), handler);
        }

        /// <summary>
        /// Send the event to the <see cref="IEventHandler{T}"/> without publishing to the EventStore.
        /// This is used when a Event has to be processed but not habe the ability to be recreated from the EventStore.
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="event"></param>
        public virtual void Send<Tevent>(Tevent @event)
        {
            _publisher.Publish(@event);
        }

        /// <summary>
        /// Publish the event to the EventStore and send the event to the <see cref="IEventHandler{T}"/>
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="event"></param>
        public void Publish<Tevent>(string id, DateTime time, Tevent @event) where Tevent : class
        {
            _eventStore.Add(Guid.NewGuid().ToString(), id, _options.StreamVersion, _options.TypeNameFactory(@event), time, @event);
            Send(@event);
        }

        /// <summary>
        /// 
        /// </summary>
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
