using System;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
    internal static class TaskFactory
    {
        public static BackgroundTask CreateTask(Expression<Action> task)
        {
            return new BackgroundTask
            {
                Task = task,
                State = TaskState.New
            };
        }

        internal static NotificationTask<T> CreateTask<T>(Expression<Func<T>> notification)
        {
            return new NotificationTask<T>
            {
                Task = notification,
                State = TaskState.New
            };
        }
    }
}
