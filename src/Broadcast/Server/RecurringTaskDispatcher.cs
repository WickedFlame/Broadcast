using System;
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

		/// <summary>
		/// Creates a new instance of the RecurringTaskDispatcher
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="store"></param>
		public RecurringTaskDispatcher(IBroadcaster broadcaster, ITaskStore store)
		{
			_broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}

		/// <summary>
		/// Execute the Dispatcher to processes the task.
		/// The Task is only processesed if it is a recurring task
		/// </summary>
		/// <param name="task"></param>
		public void Execute(ITask task)
		{
			if(task.Time != null && task.IsRecurring)
			{
				_store.Storage(s => s.Set(new StorageKey($"tasks:recurring:{task.Name}"), new RecurringTask
				{
					Id = task.Id,
					Name = task.Name, 
					NextExecution = DateTime.Now.Add(task.Time.Value)
				}));

				_broadcaster.Scheduler.Enqueue(() =>
				{
					// execute the task
					_broadcaster.Process(task);

					// clone the task for rescheduling
					_store.Add(task.Clone());
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
