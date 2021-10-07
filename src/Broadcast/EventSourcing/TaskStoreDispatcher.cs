using System;
using System.Linq;
using System.Threading;
using Broadcast.Diagnostics;
using Broadcast.Processing;
using Broadcast.Server;
using Broadcast.Storage;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Dispatcher for moving enqueued tasks and passing these to all registered dispatchers
	/// </summary>
	public class TaskStoreDispatcher : IBackgroundDispatcher<IStorageContext>
	{
		private readonly DispatcherLock _dispatcherLock;
		private readonly IStorage _storage;
		private readonly ILogger _logger;

		/// <summary>
		/// Creates a new instance of a TaskStoreDispatcher
		/// </summary>
		/// <param name="dispatcherLock"></param>
		/// <param name="storage"></param>
		public TaskStoreDispatcher(DispatcherLock dispatcherLock, IStorage storage)
		{
			_dispatcherLock = dispatcherLock ?? throw new ArgumentNullException(nameof(dispatcherLock));
			_storage = storage ?? throw new ArgumentNullException(nameof(storage));

			_logger = LoggerFactory.Create();
			_logger.Write($"Starting new TaskStoreDispatcher");
		}

		/// <summary>
		/// Execute the dispatcher and process the taskqueue
		/// </summary>
		/// <param name="context"></param>
		public void Execute(IStorageContext context)
		{
			// check if a thread is allready processing the queue
			if (_dispatcherLock.IsLocked())
			{
				return;
			}

			_dispatcherLock.Lock();

			while (_storage.TryFetchNext(new StorageKey("tasks:enqueued"), new StorageKey("tasks:dequeued"), out var id))
			{
				_logger.Write($"Dequeued task {id} for dispatchers", LogLevel.Info, Category.Log);

				// eager fetching of the data
				// first TaskStore to fetch gets to execute the Task
				var task = _storage.Get<BroadcastTask>(new StorageKey($"task:{id}"));

				if (task == null)
				{
					_storage.RemoveFromList(new StorageKey("tasks:dequeued"), id);
					_logger.Write($"Could not fetch task {id} for dispatchers because the task is not in the storage", LogLevel.Warning, Category.Log);
					continue;
				}

				//
				// Tasks can be assigned to a certain queue/server
				// that means the task has to be dipatched to the assigned queue instead of using the roundrobin to find a queue
				//
				// Check if the task is already assigned to a queue
				var serverId = _storage.GetServerIdFromQueue(task.Id);

				var dispatchers = string.IsNullOrEmpty(serverId) ?
					// use round robin to get the next set of dispatchers
					context.Dispatchers.GetNext() : 
					// get the dispatchers that are assigned to the server
					// based on the queue that the task is assigned to
					context.Dispatchers.GetNext(serverId);

				foreach (var dispatcher in dispatchers)
				{
					dispatcher.Execute(task);
				}

				if (!_storage.GetEnqueuedTasks().Any())
				{
					context.ResetEvent.Set();
				}
				else
				{
					context.ResetEvent.Reset();
				}
			}

			_dispatcherLock.Unlock();
		}
	}
}
