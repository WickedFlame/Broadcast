namespace Broadcast
{
    public static class EventBusExtensions
    {
        /// <summary>
        /// Publish the event to the EventStore and send the event to the <see cref="IEventHandler{T}"/>
        /// </summary>
        /// <typeparam name="Tevent"></typeparam>
        /// <param name="event"></param>
        public static void Publish<Tevent>(this IEventBus eventBus, Tevent @event) where Tevent : class
        {
            eventBus.Publish(Guid.NewGuid().ToString(), DateTime.Now, @event);
        }
    }
}
