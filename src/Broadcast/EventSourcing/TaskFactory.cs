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
        public static DelegateTask CreateTask(Expression<Action> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            // extract the method passed to the delegate
            var callExpression = task.Body as MethodCallExpression;
            if (callExpression == null)
            {
                throw new NotSupportedException("Expression body should be of type `MethodCallExpression`");
            }

            return new DelegateTask
            {
                Type = callExpression.Method.DeclaringType,
                Method = callExpression.Method,
                Arguments = GetArguments(callExpression),

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
        public static DelegateTask<T> CreateTask<T>(Expression<Func<T>> notification) where T : INotification
        {
            if (notification == null)
                throw new ArgumentNullException("notification");

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








        private static string[] GetArguments(MethodCallExpression callExpression)
        {
            //Debug.Assert(callExpression != null);

            //var arguments = callExpression.Arguments.Select(GetArgumentValue).ToArray();

            //var serializedArguments = new List<string>(arguments.Length);
            //foreach (var argument in arguments)
            //{
            //    string value = null;

            //    if (argument != null)
            //    {
            //        if (argument is DateTime)
            //        {
            //            value = ((DateTime)argument).ToString("o", CultureInfo.InvariantCulture);
            //        }
            //        else
            //        {
            //            value = JobHelper.ToJson(argument);
            //        }
            //    }

            //    // Logic, related to optional parameters and their default values, 
            //    // can be skipped, because it is impossible to omit them in 
            //    // lambda-expressions (leads to a compile-time error).

            //    serializedArguments.Add(value);
            //}

            //return serializedArguments.ToArray();
            return new string[0];
        }

    }
}
