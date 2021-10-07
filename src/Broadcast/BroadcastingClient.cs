using Broadcast.EventSourcing;

namespace Broadcast
{
	/// <summary>
	/// Client to enqueue new tasks to the <see cref="Broadcaster"/>
	/// </summary>
	public class BroadcastingClient : IBroadcastingClient
	{
		private readonly ITaskStore _taskStore;

		/// <summary>
		/// Creates a new instance of the BroadcastingClient.
		/// Enqueues the tasks to the default <see cref="ITaskStore"/>
		/// </summary>
		public BroadcastingClient() 
			: this(TaskStore.Default)
		{
		}

		/// <summary>
		/// Gets the associated <see cref="ITaskStore"/>
		/// </summary>
		public ITaskStore Store => _taskStore;

		/// <summary>
		/// Creates a new instance of the BroadcastingClient.
		/// Enqueues the tasks to the <see cref="ITaskStore"/>
		/// </summary>
		/// <param name="store"></param>
		public BroadcastingClient(ITaskStore store)
		{
			_taskStore = store;
		}

		/// <summary>
		/// Enqueue a new task to the <see cref="Broadcaster"/>
		/// </summary>
		/// <param name="task"></param>
		public void Enqueue(ITask task)
		{
			_taskStore.Add(task);
		}
	}
}
