
namespace Broadcast.EventSourcing
{
	/// <summary>
	/// State that the <see cref="ITask"/> can be in
	/// </summary>
    public enum TaskState
    {
        New,
        Queued,
        Dequeued,
        InProcess,
        Processed,
        Faulted
    }
}
