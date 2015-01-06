using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Broadcast.EventSourcing
{
    internal static class TaskFactory
    {
        public static DelegateTask CreateTask(Expression<Action> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

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

        public static DelegateTask<T> CreateTask<T>(Expression<Func<T>> notification)
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
