namespace Broadcast.Store
{
    public class EventStoreItem
    {
        public string Id { get; set; }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public IEvent Model { get; set; }
    }
}
