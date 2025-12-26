
namespace Broadcast
{
    public interface IEventBus : IDisposable
    {
        /// <summary>
        /// Registers the specified message handler to receive events of the given type.
        /// </summary>
        /// <remarks>Multiple handlers can be registered for the same event type. Handlers will be invoked
        /// when events of the subscribed type are published.</remarks>
        /// <typeparam name="Tevent">The type of event messages to subscribe to.</typeparam>
        /// <param name="handler">The message handler that will process incoming events of type <typeparamref name="Tevent"/>. Cannot be null.</param>
        void Subscribe<Tevent>(IMessageHandler<Tevent> handler);

        /// <summary>
        /// Send the <see cref="IEvent"/> to the <see cref="IMessageHandler{T}"/> without publishing to the EventStore.
        /// This is used when a Event has to be processed but not habe the ability to be recreated from the EventStore.
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="event"></param>
        void Send<Tevent>(Tevent @event);

        /// <summary>
        /// Publish the <see cref="IEvent"/> to the EventStore and send the <see cref="IEvent"/> to the <see cref="IMessageHandler{T}"/>
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="event"></param>
        void Publish<Tevent>(string id, DateTime time, Tevent @event) where Tevent : IEvent;
    }
}
