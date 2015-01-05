using System;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
    public class WorkerTask
    {
        public TaskState State { get; set; }
    }

    public class DelegateTask : WorkerTask
    {
        public Expression<Action> Task { get; set; }
    }

    public class DelegateTask<T> : WorkerTask
    {
        public Expression<Func<T>> Task { get; set; }
    }
}
