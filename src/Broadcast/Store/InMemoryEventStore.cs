using Broadcast.Store;

namespace Broadcast
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<EventStoreItem> _events = [];

        [Obsolete("Use Add with full metadata instead", false)]
        public string Add<T>(string testId, DateTime time, T model) where T : class
        {
            var id = Guid.NewGuid().ToString();

            _events.Add(new EventStoreItem
            {
                Id = id,
                StreamtId = testId,
                Time = time,
                Data = model
            });

            return id;
        }

        /// <summary>
        /// Adds a new event to the event store with the specified metadata and data payload.
        /// </summary>
        /// <typeparam name="T">The type of the event data to store. Must be a reference type.</typeparam>
        /// <param name="eventId">The unique identifier for the event to add. Cannot be null.</param>
        /// <param name="streamId">The identifier of the stream to which the event belongs. Cannot be null.</param>
        /// <param name="streamVersion">The version number of the stream after this event is added. Must be a non-negative integer.</param>
        /// <param name="type">The type or category of the event. Cannot be null.</param>
        /// <param name="time">The timestamp indicating when the event occurred.</param>
        /// <param name="data">The event data to store. Must not be null.</param>
        /// <returns>The unique identifier of the event that was added.</returns>
        public string Add<T>(string eventId, string streamId, int streamVersion, string type, DateTime time, T data) where T : class
        {
            _events.Add(new EventStoreItem
            {
                Id = eventId,
                StreamtId = streamId,
                StreamVersion = streamVersion,
                Type = type,
                Time = time,
                Data = data
            });
            return eventId;
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
                // do stuf here
            }
        }
    }
}
