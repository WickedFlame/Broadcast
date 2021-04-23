using System;
using System.Collections.Generic;
using Broadcast.Scheduling;

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
		/// <param name="task">The task to schedule</param>
		/// <param name="time">The time to execute the task at</param>
		void Enqueue(Action task, TimeSpan time);
	}
}
