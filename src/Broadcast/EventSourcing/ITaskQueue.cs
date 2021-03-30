
namespace Broadcast.EventSourcing
{
	/// <summary>
	/// The TaskQueue
	/// </summary>
	public interface ITaskQueue
	{
		/// <summary>
		/// gets the amount of items in the queue
		/// </summary>
		int Count { get; }

		/// <summary>
		/// enqueue a new event
		/// </summary>
		/// <param name="event"></param>
		void Enqueue(ITask @event);

		/// <summary>
		/// try to dequeue a event.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool TryDequeue(out ITask value);
	}
}
