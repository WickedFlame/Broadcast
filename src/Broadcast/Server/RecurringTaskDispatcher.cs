using System;
using Broadcast.Diagnostics;
using Broadcast.EventSourcing;
using Broadcast.Storage;

namespace Broadcast.Server
{
	/// <summary>
	/// Dispatcher that dispatches recurring tasks to the <see cref="IScheduler"/> and the <see cref="IBroadcaster"/> to be processed
	/// </summary>
	public class RecurringTaskDispatcher : IDispatcher
	{
		private readonly IBroadcaster _broadcaster;
		private readonly ITaskStore _store;
		private readonly ILogger _logger;

		/// <summary>
		/// Creates a new instance of the RecurringTaskDispatcher
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="store"></param>
		public RecurringTaskDispatcher(IBroadcaster broadcaster, ITaskStore store)
		{
			_broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
			_store = store ?? throw new ArgumentNullException(nameof(store));
			_logger = LoggerFactory.Create();
		}

		/// <summary>
		/// Execute the Dispatcher to processes the task.
		/// The Task is only processesed if it is a recurring task
		/// </summary>
		/// <param name="task"></param>
		public void Execute(ITask task)
		{
			if (task.Time != null && task.IsRecurring)
			{
				if (task.State == TaskState.Deleted)
				{
					_logger.Write($"Task {task.Id} is marked as deleted and will not be processed by the RecurringTaskDispatcher", LogLevel.Warning);
					return;
				}

				_store.Storage(s => s.Set(new StorageKey($"tasks:recurring:{task.Name}"), new RecurringTask
				{
					ReferenceId = task.Id,
					Name = task.Name, 
					NextExecution = DateTime.Now.Add(task.Time.Value),
					Interval = task.Time
				}));

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

				_broadcaster.Scheduler.Enqueue(task.Id, id =>
				{
					// reload the task to get all changes since being enqueued in the scheduler
					var stored = _store.Storage(s => s.Get<BroadcastTask>(new StorageKey($"task:{id}")));
					if (stored == null)
					{
						_logger.Write($"Schedlued Task {id} could not be processed because it does not exist in the Storage", LogLevel.Warning);
						return;
					}

					if (stored.State == TaskState.Deleted)
					{
						_logger.Write($"Scheduled Task {id} is marked as deleted and will not be processed", LogLevel.Warning);
						return;
					}

					// execute the task
					_broadcaster.Process(stored);

					// remove the task from the queue
					_store.Storage(s => s.RemoveFromList(new StorageKey($"queue:{_broadcaster.Name}"), id));

					// clone the task for rescheduling
					_store.Add(stored.Clone());
				}, task.Time ?? TimeSpan.Zero);
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
