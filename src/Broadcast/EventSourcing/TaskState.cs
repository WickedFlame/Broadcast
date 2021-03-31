
namespace Broadcast.EventSourcing
{
    public enum TaskState
    {
        New,
        Queued,
        Dequeued,
        InProcess,
        Processed
    }
}
