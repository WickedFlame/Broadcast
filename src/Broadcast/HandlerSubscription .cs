namespace Broadcast
{
    /// <summary>
    /// 
    /// </summary>
    public class HandlerSubscription
    {
        private readonly Func<object, object> _handle;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public HandlerSubscription(Type eventType, IMessageHandler handler)
        {
            EventType = eventType;
            Handler = handler;

            var meth = Handler.GetType().GetMethod("Handle", [eventType]);
            _handle = o => meth.Invoke(Handler, [o]);
        }

        /// <summary>
        /// Gets the type of the event associated with this instance.
        /// </summary>
        public Type EventType { get; }

        /// <summary>
        /// Gets the message handler used to process incoming messages.
        /// </summary>
        public IMessageHandler Handler { get; }

        /// <summary>
        /// Attempts to handle the specified event, suppressing any exceptions that occur during handling.
        /// </summary>
        /// <typeparam name="T">The type of the event to handle.</typeparam>
        /// <param name="event">The event instance to be handled. This value can be null if the handler supports null events.</param>
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
