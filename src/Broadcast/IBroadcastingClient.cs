using Broadcast.EventSourcing;

namespace Broadcast
{
	/// <summary>
	/// Client to enqueue new tasks to the <see cref="Broadcaster"/>
	/// </summary>
	public interface IBroadcastingClient
	{
		/// <summary>
		/// Enqueue a new task to the <see cref="Broadcaster"/>
		/// </summary>
		/// <param name="task"></param>
		void Enqueue(ITask task);
	}
}
