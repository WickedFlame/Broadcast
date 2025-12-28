namespace Broadcast
{
    //
    // Dispatcher uses a Queue to process data async
    // This allows the dispatcher to process big amounts of data
    //

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Dispatcher<T> : IDispatcher<T>
    {
        private readonly object _lock = new object();

        private readonly HandlerSubscriptionCollection _handlers = [];
        private readonly Queue<T> _queue = new();
        private readonly IEventBus _eventBus;
        private readonly TimedDispatcher _dispatcher;

        private readonly EventPublisher _publisher;

        /// <summary>
        /// 
        /// </summary>
        public Dispatcher()
            : this(new EventBus())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventBus"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Dispatcher(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException("eventBus");

            _dispatcher = new(5000, () => DispatcherTask());
            _dispatcher.StartDispatcher();

            _publisher = new EventPublisher(_handlers);
        }

        /// <summary>
        /// Gets a enumeration of all registered message handlers.
        /// </summary>
        public IEnumerable<IMessageHandler> Handlers => _handlers.Select(h => h.Handler);

        /// <summary>
        /// Gets the current queue of events to be processed.
        /// </summary>
        public IEnumerable<T> Queue => _queue;

        /// <summary>
        /// Registers a message handler for messages of the specified type.
        /// </summary>
        /// <remarks>If a handler for the specified message type is already registered, this method may
        /// replace or ignore the existing handler depending on the implementation.</remarks>
        /// <typeparam name="Tc">The type of message to handle. Must be a reference type that implements or derives from T.</typeparam>
        /// <param name="handler">The message handler to register for messages of type Tc. Cannot be null.</param>
        public void Register<Tc>(IMessageHandler<Tc> handler) where Tc : class, T
        {
            _handlers.Add(typeof(Tc), handler);
        }


        private bool DispatcherTask()
        {
            var entry = GetNext();
            while (entry != null)
            {
                Send(entry);

                entry = GetNext();

                if (!_dispatcher.IsRunning)
                {
                    return false;
                }
            }

            return true;
        }

        private T GetNext()
        {
            lock (_lock)
            {
                return _queue.Count > 0 ? _queue.Dequeue() : default;
            }
        }

        [Obsolete("Use Enqueue instead", false)]
        public void SendAsync<Tc>(Tc @event) where Tc : class, T
        {
            Enqueue(@event);
        }

        /// <summary>
        /// Enqueue the event to be processed by the dispatcher in the background in a async dispatcher.
        /// </summary>
        /// <typeparam name="Tc"></typeparam>
        /// <param name="event"></param>
        public void Enqueue<Tc>(Tc @event) where Tc : class, T
        {
            lock (_lock)
            {
                _queue.Enqueue(@event);
            }

            _dispatcher.Continue();
        }

        /// <summary>
        /// Publishes the specified event to all registered subscribers.
        /// </summary>
        /// <typeparam name="Tevent">The type of the event to send.</typeparam>
        /// <param name="event">The event instance to publish. Cannot be null.</param>
        public void Send<Tevent>(Tevent @event)
        {
            if (_publisher.Publish(@event))
            {
                return;
            }

            if (@event is IEvent evt)
            {
                _eventBus.Publish(Guid.NewGuid().ToString(), DateTime.UtcNow, evt);
            }
        }

        /// <summary>
        /// Close the dispatcher and stop processing messages.
        /// </summary>
        public void Close()
        {
            _dispatcher.IsRunning = false;
            _dispatcher.Continue();
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
                Close();
                foreach (var handler in _handlers)
                {
                    handler.Handler?.Dispose();
                }
            }
        }
    }
}
