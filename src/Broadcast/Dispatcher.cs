namespace Broadcast
{
    //
    // Dispatcher uses a Queue to process data async
    // This allows the dispatcher to process big amounts of data
    //

    public class Dispatcher<T> : IDispatcher<T>, IDisposable
    {
        private readonly object _lock = new object();

        private readonly Dictionary<Type, IMessageHandler> _handlers = [];
        private readonly Queue<T> _queue = new();
        private readonly TimedDispatcher _dispatcher;

        public Dispatcher()
        {
            _dispatcher = new(5000, () => DispatcherTask());
            _dispatcher.StartDispatcher();
        }

        public void Register<Tc>(IMessageHandler<T> handler) where Tc : class, T
        {
            _handlers[typeof(Tc)] = handler;
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

        public void SendAsync<Tc>(Tc @event) where Tc : class, T
        {
            lock (_lock)
            {
                _queue.Enqueue(@event);
            }

            _dispatcher.Continue();
        }


        public void Send<Tevent>(Tevent @event)
        {
            var handler = _handlers[@event.GetType()] as IMessageHandler<Tevent>;
            if (handler == null)
            {
                return;
            }

            handler.Handle(@event);
        }

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
                    handler.Value?.Dispose();
                }
            }
        }
    }
}
