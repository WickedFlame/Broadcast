using System;

namespace Broadcast
{
    public class SchedulerTask
    {
        public SchedulerTask(Action task, TimeSpan time)
        {
            Task = task;
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
