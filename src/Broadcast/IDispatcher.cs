namespace Broadcast
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDispatcher<T> : IDisposable
    {
        /// <summary>
        /// Registers a message handler for messages of the specified type.
        /// </summary>
        /// <remarks>If a handler for the specified message type is already registered, this method may
        /// replace or ignore the existing handler depending on the implementation.</remarks>
        /// <typeparam name="Tc">The type of message to handle. Must be a reference type that implements or derives from T.</typeparam>
        /// <param name="handler">The message handler to register for messages of type Tc. Cannot be null.</param>
        void Register<Tc>(IMessageHandler<Tc> handler) where Tc : class, T;

        /// <summary>
        /// Publishes the specified event to all registered subscribers.
        /// </summary>
        /// <typeparam name="Tevent">The type of the event to send.</typeparam>
        /// <param name="event">The event instance to publish. Cannot be null.</param>
        void Send<Tevent>(Tevent @event);

        [Obsolete("Use Enqueue instead", false)]
        void SendAsync<Tc>(Tc @event) where Tc : class, T;

        /// <summary>
        /// Enqueue the event to be processed by the dispatcher in a async dispatcher.
        /// </summary>
        /// <typeparam name="Tc"></typeparam>
        /// <param name="event"></param>
        void Enqueue<Tc>(Tc @event) where Tc : class, T;

        /// <summary>
        /// Close the dispatcher and stop processing messages.
        /// </summary>
        void Close();
    }
}
