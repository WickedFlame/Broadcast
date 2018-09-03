using System;
using System.Linq.Expressions;

namespace Broadcast
{
    public class SchedulerTask
    {
        public SchedulerTask(Action task, TimeSpan time)
        {
            Task = task;
            Time = time;
        }

        public Action Task { get; }

        public TimeSpan Time { get; }
    }
}
