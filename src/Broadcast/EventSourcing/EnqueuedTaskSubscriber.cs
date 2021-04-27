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
			_store.Storage(s =>
			{
				var taskIds = s.GetList<string>(new StorageKey("tasks:enqueued"));
				while (taskIds.Any())
				{
					var id = taskIds.FirstOrDefault();

					// local tasks like actions are not serializeable
					// these are automaticaly propagated to the dispatchers for processing
					if (s.RemoveFromList(new StorageKey("tasks:enqueued"), id))
					{
						s.AddToList(new StorageKey("tasks:dequeued"), id);
						var task = s.Get<ITask>(new StorageKey($"task:{id}"));
						_store.DispatchTask(task);
					}

					taskIds = s.GetList<string>(new StorageKey("tasks:enqueued"));
				}
			});
		}
	}
}
