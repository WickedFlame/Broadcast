using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Broadcast.EventSourcing
{
    public abstract class BroadcastTask
    {
        public TaskState State { get; set; }

        //public Type Type { get; set; }

        //public MethodInfo Method { get; set; }

        //public string[] Arguments { get; set; }
        
        internal abstract void CloseTask();
    }

    public class DelegateTask : BroadcastTask
    {
        public Action Task { get; set; }

        internal override void CloseTask()
        {
            Task = null;
        }
    }

    public class DelegateTask<T> : BroadcastTask
    {
        public Func<T> Task { get; set; }

        internal override void CloseTask()
        {
            Task = null;
        }
    }
}
