using System;
using System.Collections.Generic;

namespace Broadcast
{
	/// <summary>
	/// Interface for the scheduler
	/// </summary>
	public interface IScheduler : IDisposable
	{
		/// <summary>
		/// Gets the elapsed time since the Scheduler has been started
		/// </summary>
		TimeSpan Elapsed { get; }

		/// <summary>
		/// Gets the Queue of scheduled tasks
		/// </summary>
		IEnumerable<SchedulerTask> GetActiveTasks();

		/// <summary>
		/// Enqueues and schedules a new task
		/// </summary>
		/// <param name="task">The task to schedule</param>
		/// <param name="time">The time to execute the task at</param>
		void Enqueue(Action task, TimeSpan time);

		/// <summary>
		/// Removes the task from the schedule queue
		/// </summary>
		/// <param name="task">The task to remove</param>
		void Dequeue(SchedulerTask task);
	}
}
