using System.Collections;

namespace Broadcast
{
    /// <summary>
    /// Represents a collection of <see cref="HandlerSubscription"/> instances.
    /// </summary>
    public class HandlerSubscriptionCollection : IEnumerable<HandlerSubscription>
    {
        private readonly List<HandlerSubscription> _subscriptions = new List<HandlerSubscription>();

        /// <summary>
        /// Registers a message handler for the specified message type.
        /// </summary>
        /// <param name="type">The type of message to associate with the handler. Typically, this is the message class or interface the
        /// handler processes.</param>
        /// <param name="handler">The handler instance that will process messages of the specified type. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
        public void Add(Type type, IMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Add(new HandlerSubscription(type, handler));
        }

        /// <summary>
        /// Adds a new <see cref="HandlerSubscription"/> to the collection.
        /// </summary>
        /// <param name="subscription">The subscription to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="subscription"/> is null.</exception>
        public void Add(HandlerSubscription subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            _subscriptions.Add(subscription);
        }

        /// <summary>
        /// Retrieves all handler subscriptions associated with the specified event type.
        /// </summary>
        /// <param name="eventType">The event type for which to retrieve handler subscriptions. Cannot be null.</param>
        /// <returns>An enumerable collection of <see cref="HandlerSubscription"/> objects that are registered for the specified
        /// event type. The collection is empty if no subscriptions are found.</returns>
        public IEnumerable<HandlerSubscription> Get(Type eventType)
        {
            foreach (var subscription in _subscriptions)
            {
                if (subscription.EventType == eventType)
                {
                    yield return subscription;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<HandlerSubscription> GetEnumerator()
        {
            return _subscriptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}