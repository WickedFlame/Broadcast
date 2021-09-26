using System;

namespace Broadcast
{
	/// <summary>
	/// Interface for the scheduler
	/// </summary>
	public interface IScheduler : IDisposable
	{
		/// <summary>
		/// Enqueues and schedules a new task
		/// </summary>
		/// <param name="id">The id of the <see cref="Broadcast.EventSourcing.ITask"/></param>
		/// <param name="task">The task to schedule</param>
		/// <param name="time">The time to execute the task at</param>
		void Enqueue(string id, Action<string> task, TimeSpan time);
	}
}
