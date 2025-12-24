using System.Reflection;

namespace Broadcast
{
    public class EventBus : IEventBus
    {
        private readonly List<MessageHandlerRegistration> _handlers = [];
        private readonly IEventStore _eventStore;

        public EventBus(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void Subscribe<Tevent>(IMessageHandler<Tevent> handler)
        {
            _handlers.Add(new MessageHandlerRegistration(typeof(Tevent), handler));
        }

        /// <summary>
        /// Send the <see cref="IEvent"/> to the <see cref="IMessageHandler{T}"/>
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

    public class MessageHandlerRegistration
    {
        private readonly Func<object, object> _handle;

        public MessageHandlerRegistration(Type eventType, IMessageHandler handler)
        {
            EventType = eventType;
            Handler = handler;

            var meth = Handler.GetType().GetMethod("Handle", [eventType]);
            _handle = o => meth.Invoke(Handler, [o]);
        }

        public Type EventType { get; }

        public IMessageHandler Handler { get; }

        public void TryHandle<T>(T @event)
        {
            try
            {
                _handle(@event);
            }
            catch
            {
                // do nothing
            }
        }
    }
}
