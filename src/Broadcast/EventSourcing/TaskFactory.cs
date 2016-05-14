using System;
using System.Linq.Expressions;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Factory class that creates BroadcastTasks based on the delegate
    /// </summary>
    public static class TaskFactory
    {
        /// <summary>
        /// Creates a DelegateTask based on the Action delegate
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static DelegateTask CreateTask(Action task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            return new DelegateTask
            {
                Task = task,
                State = TaskState.New
            };
        }

        /// <summary>
        /// Creates a Notification Task based on the Func delegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        /// <returns></returns>
        public static DelegateTask<T> CreateNotifiableTask<T>(Func<T> notification) where T : INotification
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            //var callExpression = notification.Body as MethodCallExpression;
            //if (callExpression == null)
            //{
            //    throw new NotSupportedException("Expression body should be of type `MethodCallExpression`");
            //}

            return new DelegateTask<T>
            {
                //Type = typeof(T),
                //Method = callExpression.Method,
                //Arguments = GetArguments(callExpression),

                Task = notification,
                State = TaskState.New
            };
        }

        public static DelegateTask<T> CreateTask<T>(Func<T> process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }
            
            return new DelegateTask<T>
            {
                Task = process,
                State = TaskState.New
            };
        }
    }
}
