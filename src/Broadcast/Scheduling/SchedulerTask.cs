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
		/// <param name="id"></param>
		/// <param name="task"></param>
		/// <param name="time"></param>
		public SchedulerTask(string id, Action<string> task, TimeSpan time)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			Task = task ?? throw new ArgumentNullException(nameof(task));
            Time = time;
        }

		/// <summary>
		/// Gets the id of the <see cref="Broadcast.EventSourcing.ITask"/>
		/// </summary>
		public string Id{ get; }

        /// <summary>
        /// Gets the Task to execute at the scheduled time
        /// </summary>
        public Action<string> Task { get; }

        /// <summary>
        /// Gets the time that the Task has to be executed at
        /// </summary>
        public TimeSpan Time { get; }
    }
}
