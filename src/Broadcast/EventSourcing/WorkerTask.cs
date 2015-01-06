using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Broadcast.EventSourcing
{
    public abstract class WorkerTask
    {
        public TaskState State { get; set; }

        public Type Type { get; set; }
        public MethodInfo Method { get; set; }

        public string[] Arguments { get; set; }
        
        internal abstract void CloseTask();
    }

    public class DelegateTask : WorkerTask
    {
        public Expression<Action> Task { get; set; }

        internal override void CloseTask()
        {
            Task = null;
        }
    }

    public class DelegateTask<T> : WorkerTask
    {
        public Expression<Func<T>> Task { get; set; }

        internal override void CloseTask()
        {
            Task = null;
        }
    }
}
