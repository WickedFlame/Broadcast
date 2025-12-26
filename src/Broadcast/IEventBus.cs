
namespace Broadcast
{
    public interface IEventBus : IDisposable
    {
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
