using Broadcast.Store;
using System.IO;

namespace Broadcast
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<EventStoreItem> _events = [];

        /// <summary>
        /// Gets the collection of event items stored in the event store.
        /// </summary>
        public IEnumerable<EventStoreItem> Events => _events;

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
        public Task<AppendResult> AddAsync<T>(string eventId, string streamId, int streamVersion, string type, DateTime time, T data) where T : class
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

            return Task.FromResult(new AppendResult { EventId = eventId, StreamId = streamId, Success = true });
        }

        /// <summary>
        /// Asynchronously retrieves all events from the specified stream.
        /// </summary>
        /// <param name="streamId">The unique identifier of the stream whose events are to be read. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
        /// cref="EventEnvelope"/> objects for the specified stream. The collection is empty if the stream does not
        /// exist or contains no events.</returns>
        public Task<IEnumerable<EventEnvelope>> ReadStreamAsync(string streamId)
        {
            return Task.FromResult(_events
                .Where(e => e.StreamtId == streamId)
                .Select(e => new EventEnvelope
                {
                    Id = e.Id,
                    StreamId = e.StreamtId,
                    StreamVersion = e.StreamVersion,
                    Type = e.Type,
                    Time = e.Time,
                    Data = e.Data
                }));
        }

        /// <summary>
        /// Asynchronously retrieves all events as a collection of event envelopes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see
        /// cref="EventEnvelope"/> objects representing all events. The collection will be empty if no events are
        /// available.</returns>
        public Task<IEnumerable<EventEnvelope>> ReadAllAsync()
        {
            return Task.FromResult(_events
                .Select(e => new EventEnvelope
                {
                    Id = e.Id,
                    StreamId = e.StreamtId,
                    StreamVersion = e.StreamVersion,
                    Type = e.Type,
                    Time = e.Time,
                    Data = e.Data
                }));
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
