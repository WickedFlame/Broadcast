namespace Broadcast
{
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
