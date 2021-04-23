using System.Collections.Generic;

namespace Broadcast.Scheduling
{
	/// <summary>
	/// Threadsafe Queue for managing <see cref="SchedulerTask"/>
	/// </summary>
	public interface IScheduleQueue
	{
		/// <summary>
		/// Adds a new task to the queue
		/// </summary>
		/// <param name="task"></param>
		void Enqueue(SchedulerTask task);

		/// <summary>
		/// Removes the task from the schedule queue
		/// </summary>
		/// <param name="task">The task to remove</param>
		void Dequeue(SchedulerTask task);

		/// <summary>
		/// Creates a copy of the queue
		/// </summary>
		/// <returns></returns>
		IEnumerable<SchedulerTask> ToList();
	}
}
