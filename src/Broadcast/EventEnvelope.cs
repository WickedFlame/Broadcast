namespace Broadcast
{
    public class EventEnvelope
    {
        public string Id { get; set; }
        
        public string StreamId { get; set; }

        public int StreamVersion { get; set; }

        public DateTime Time { get; set; }

        public string Type { get; set; }

        public object Data { get; set; }
    }
}
