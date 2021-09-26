using System;
using Broadcast.EventSourcing;
using Broadcast.Storage;

namespace Broadcast.Server
{
	/// <summary>
	/// Dispatcher that dispatches scheduled tasks to the <see cref="IScheduler"/> and the <see cref="IBroadcaster"/> to be processed
	/// </summary>
	public class ScheduleTaskDispatcher : IDispatcher
	{
		private readonly IBroadcaster _broadcaster;
		private readonly ITaskStore _store;

		/// <summary>
		/// Creates a new instance of the ScheduleTaskDispatcher
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="store"></param>
		public ScheduleTaskDispatcher(IBroadcaster broadcaster, ITaskStore store)
		{
			_broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}

		/// <summary>
		/// Execute the Dispatcher to processes the task.
		/// The Task is only processesed if it is a scheduled task
		/// </summary>
		/// <param name="task"></param>
		public void Execute(ITask task)
		{
			if (task.Time != null && !task.IsRecurring)
			{
				_broadcaster.Scheduler.Enqueue(task.Id, id =>
				{
					// reload the task to get all changes since being enqueued in the scheduler
					var stored = _store.Storage(s => s.Get<BroadcastTask>(new StorageKey($"task:{id}")));

					_broadcaster.Process(stored);
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
