using System;
using Broadcast.Configuration;
using Broadcast.EventSourcing;

namespace Broadcast
{
	/// <summary>
	/// Client to enqueue new tasks to the <see cref="Broadcaster"/>
	/// </summary>
	public class BroadcastingClient : IBroadcastingClient
	{
		private static readonly ItemFactory<IBroadcastingClient> ItemFactory = new ItemFactory<IBroadcastingClient>(() => new BroadcastingClient());

		/// <summary>
		/// Gets the default instance of the <see cref="IBroadcastingClient"/>
		/// </summary>
		public static IBroadcastingClient Default => ItemFactory.Factory();

		/// <summary>
		/// Setup a new instance for the default <see cref="IBroadcastingClient"/>.
		/// Setup with null to reset to the default
		/// </summary>
		/// <param name="setup"></param>
		public static void Setup(Func<IBroadcastingClient> setup)
		{
			ItemFactory.Factory = setup;
		}

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
