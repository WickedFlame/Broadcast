using System;

namespace Broadcast.Scheduling
{
	/// <summary>
	/// Task that is stored in the <see cref="IScheduler"/>.
	/// The task/action gets executed when the time is reached.
	/// </summary>
    public class SchedulerTask
    {
		/// <summary>
		/// Creates a new instance of the SchedulerTask
		/// </summary>
		/// <param name="task"></param>
		/// <param name="time"></param>
		public SchedulerTask(Action task, TimeSpan time)
		{
			Task = task ?? throw new ArgumentNullException(nameof(task));
            Time = time;
        }

        /// <summary>
        /// Gets the Task to execute at the scheduled time
        /// </summary>
        public Action Task { get; }

        /// <summary>
        /// Gets the time that the Task has to be executed at
        /// </summary>
        public TimeSpan Time { get; }
    }
}
