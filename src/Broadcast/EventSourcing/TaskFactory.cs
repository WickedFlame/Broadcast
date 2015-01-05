using System;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
    internal static class TaskFactory
    {
        public static DelegateTask CreateTask(Expression<Action> task)
        {
            return new DelegateTask
            {
                Task = task,
                State = TaskState.New
            };
        }

        public static DelegateTask<T> CreateTask<T>(Expression<Func<T>> notification)
        {
            return new DelegateTask<T>
            {
                Task = notification,
                State = TaskState.New
            };
        }
    }
}
