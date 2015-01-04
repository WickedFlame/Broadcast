using System;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
    public class WorkerTask
    {
        public TaskState State { get; set; }
    }

    public class BackgroundTask : WorkerTask
    {
        public Expression<Action> Task { get; set; }

        public TaskState State { get; set; }
    }

    public class NotificationTask<T> : WorkerTask
    {
        public Expression<Func<T>> Task { get; set; }

        public TaskState State { get; set; }
    }
}
