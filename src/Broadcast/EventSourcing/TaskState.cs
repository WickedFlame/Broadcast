
namespace Broadcast.EventSourcing
{
	/// <summary>
	/// State that the <see cref="ITask"/> can be in
	/// </summary>
    public enum TaskState
    {
		/// <summary>
		/// New Task
		/// </summary>
        New,

		/// <summary>
		/// Task is queued
		/// </summary>
        Queued,

		/// <summary>
		/// Task is dequeued
		/// </summary>
        Dequeued,

		/// <summary>
		/// Task is being processed
		/// </summary>
        InProcess,

		/// <summary>
		/// Task is processed
		/// </summary>
        Processed,

		/// <summary>
		/// Task is failed
		/// </summary>
        Faulted
    }
}
