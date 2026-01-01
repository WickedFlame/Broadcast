namespace Broadcast
{
    public class EventStoreOptions
    {
        public int StreamVersion { get; set; } = 1;

        public Func<object, string> TypeNameFactory { get; set; } = (e) => e.GetType().FullName;
    }
}
