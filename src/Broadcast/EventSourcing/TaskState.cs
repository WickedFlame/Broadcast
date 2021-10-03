
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
        New = 0,

		/// <summary>
		/// Task is queued
		/// </summary>
        Queued = 1,

		/// <summary>
		/// Task is dequeued
		/// </summary>
        Dequeued = 2,

		/// <summary>
		/// Task is being processed
		/// </summary>
        Processing = 3,

		/// <summary>
		/// Task is processed
		/// </summary>
        Processed = 4,

		/// <summary>
		/// Task is failed
		/// </summary>
        Faulted = 5,

		/// <summary>
		/// Task is deleted
		/// </summary>
		Deleted = 6
    }
}
