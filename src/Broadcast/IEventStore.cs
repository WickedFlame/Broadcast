
namespace Broadcast
{
    /// <summary>
    /// Defines the contract for an event store that supports appending and retrieving events with associated metadata
    /// and payloads.
    /// </summary>
    public interface IEventStore : IDisposable
    {
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
        Task<AppendResult> AddAsync<T>(string eventId, string streamId, int streamVersion, string type, DateTime time, T data) where T : class;

        /// <summary>
        /// Asynchronously retrieves all events from the specified stream.
        /// </summary>
        /// <param name="streamId">The unique identifier of the stream whose events are to be read. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
        /// cref="EventEnvelope"/> objects for the specified stream. The collection is empty if the stream does not
        /// exist or contains no events.</returns>
        Task<IEnumerable<EventEnvelope>> ReadStreamAsync(string streamId);

        /// <summary>
        /// Asynchronously retrieves all events as a collection of event envelopes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see
        /// cref="EventEnvelope"/> objects representing all events. The collection will be empty if no events are
        /// available.</returns>
        Task<IEnumerable<EventEnvelope>> ReadAllAsync();
    }
}
