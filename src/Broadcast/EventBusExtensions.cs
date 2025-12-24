namespace Broadcast
{
    public static class EventBusExtensions
    {
        /// <summary>
        /// Publish the <see cref="IEvent"/> to the EventStore and send the <see cref="IEvent"/> to the <see cref="IMessageHandler{T}"/>
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="event"></param>
        public static void Publish<Tevent>(this IEventBus eventBus, Tevent @event) where Tevent : IEvent
        {
            eventBus.Publish(Guid.NewGuid().ToString(), DateTime.Now, @event);
        }
    }
}
