using Broadcast.Store;

namespace Broadcast
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<EventStoreItem> _events = [];

        public string Add<T>(string testId, DateTime time, T model) where T : IEvent
        {
            var id = Guid.NewGuid().ToString();

            _events.Add(new EventStoreItem
            {
                Id = id,
                TestId = testId,
                Time = time,
                Model = model
            });

            return id;
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
