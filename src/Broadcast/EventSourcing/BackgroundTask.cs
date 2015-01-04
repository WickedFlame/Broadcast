using System;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
    public class BackgroundTask
    {
        public Expression<Action> Task { get; set; }

        public TaskState State { get; set; }
    }
}
