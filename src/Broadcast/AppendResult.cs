namespace Broadcast
{
    public class AppendResult
    {
        public string EventId { get; set; }

        public string StreamId { get; set; }

        public bool Success { get; set; }
    }
}
