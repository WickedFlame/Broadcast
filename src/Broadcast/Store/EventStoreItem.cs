namespace Broadcast.Store
{
    public class EventStoreItem
    {
        public string Id { get; set; }

        public string StreamtId { get; set; }

        public int StreamVersion { get; set; }

        public string Type { get; set; }

        public DateTime Time { get; set; }

        public object Data { get; set; }
    }
}
