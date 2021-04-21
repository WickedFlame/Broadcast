using System;
using Broadcast.EventSourcing;

namespace Broadcast.Server
{
	/// <summary>
	/// Dispatcher that dispatches scheduled tasks to the <see cref="IScheduler"/> and the <see cref="IBroadcaster"/> to be processed
	/// </summary>
	public class ScheduleTaskDispatcher : IDispatcher
	{
		private readonly IBroadcaster _broadcaster;

		/// <summary>
		/// Creates a new instance of the ScheduleTaskDispatcher
		/// </summary>
		/// <param name="broadcaster"></param>
		public ScheduleTaskDispatcher(IBroadcaster broadcaster)
		{
			_broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
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
				_broadcaster.Scheduler.Enqueue(() => _broadcaster.Process(task), task.Time ?? TimeSpan.Zero);
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
