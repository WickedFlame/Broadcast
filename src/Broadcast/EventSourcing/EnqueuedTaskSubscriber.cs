using System.Linq;
using Broadcast.Storage;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Subscription for the <see cref="IStorage"/> that propagates a added <see cref="ITask"/> to the <see cref="ITaskStore"/>
	/// </summary>
	public class EnqueuedTaskSubscriber : ISubscription
	{
		private readonly ITaskStore _store;

		/// <summary>
		/// Creates a new instance of EnqueuedTaskSubscriber
		/// </summary>
		/// <param name="store"></param>
		public EnqueuedTaskSubscriber(ITaskStore store)
		{
			_store = store ?? throw new System.ArgumentNullException(nameof(store));
		}

		/// <inheritdoc/>
		public string EventKey { get; } = "task";

		/// <summary>
		/// Notifies the storage that a <see cref="ITask"/> was enqueued and dispatches this to all registered <see cref="IDispatcher"/> in the <see cref="ITaskStore"/>
		/// The subsciber uses a greedy
		/// </summary>
		public void RaiseEvent()
		{
			_store.DispatchTasks();
		}
	}
}
