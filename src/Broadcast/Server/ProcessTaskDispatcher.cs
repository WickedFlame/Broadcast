using System;
using Broadcast.Diagnostics;
using Broadcast.EventSourcing;
using Broadcast.Storage;

namespace Broadcast.Server
{
	/// <summary>
	/// Dispatcher that dispatches tasks to the <see cref="IBroadcaster"/> to be processed
	/// </summary>
	public class ProcessTaskDispatcher : IDispatcher
	{
		private readonly IBroadcaster _broadcaster;
		private readonly ILogger _logger;
		private ITaskStore _store;

		/// <summary>
		/// Creates a new instance of the ProcessTaskDispatcher
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="store"></param>
		public ProcessTaskDispatcher(IBroadcaster broadcaster, ITaskStore store)
		{
			_broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
			_store = store ?? throw new ArgumentNullException(nameof(store));
			_logger = LoggerFactory.Create();
		}

		/// <summary>
		/// Execute the Dispatcher to processes the task.
		/// The Task is only processesed if it is not a scheduled task and is not a recurring task
		/// </summary>
		/// <param name="task"></param>
		public void Execute(ITask task)
		{
			if(task.Time == null && !task.IsRecurring)
			{
				if (task.State == TaskState.Deleted)
				{
					_logger.Write($"Task {task.Id} is marked as deleted and will not be processed by the ProcessTaskDipatcher", LogLevel.Warning);
					return;
				}

				//TODO: Update Recurring - set the servername where the queue is working on
				//this is equal to the queuename
				//Queue: [servername]
				_store.Storage(s =>
				{
					// set the servername where the queue is working on
					s.SetValues(new StorageKey($"tasks:values:{task.Id}"), new DataObject
					{
						{"Queue", _broadcaster.Name}
					});

					// assign the task to the queue
					s.AddToList(new StorageKey($"queue:{_broadcaster.Name}"), task.Id);
				});

				_broadcaster.Process(task);

				// remove the task from the queue
				_store.Storage(s => s.RemoveFromList(new StorageKey($"queue:{_broadcaster.Name}"), task.Id));
			}
		}

		/// <summary>
		/// Dispose the Dispatcher
		/// </summary>
		public void Dispose()
		{
		}
	}
}
