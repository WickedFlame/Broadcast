namespace Broadcast
{
    public class EventEnvelope
    {
        public string Id { get; internal set; }
        
        public string StreamId { get; set; }

        public int StreamVersion { get; internal set; }

        public DateTime Time { get; internal set; }

        public string Type { get; internal set; }

        public object Data { get; internal set; }
    }
}
