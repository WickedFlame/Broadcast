
namespace Broadcast.EventSourcing
{
    public enum TaskState
    {
        New,
        Queued,
        InProcess,
        Processed,
    }
}
