﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Broadcast.EventSourcing;

namespace Broadcast.Composition
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
			{
				throw new ArgumentNullException(nameof(task));
			}

			//return new DelegateTask
			//{
			//    Task = task,
			//    State = TaskState.New
			//};

			return CreateTaskFromExpression(task);
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
				throw new ArgumentNullException(nameof(notification));
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
				throw new ArgumentNullException(nameof(process));
			}

			return new DelegateTask<T>
			{
				Task = process,
				State = TaskState.New
			};
		}












		private static DelegateTask CreateTaskFromExpression(LambdaExpression methodCall)
		{
			if (methodCall == null)
			{
				throw new ArgumentNullException(nameof(methodCall));
			}

			if (!(methodCall.Body is MethodCallExpression callExpression))
			{
				throw new ArgumentException("Expression body should be of type `MethodCallExpression`", nameof(methodCall));
			}

			var type = callExpression.Method.DeclaringType;
			var method = callExpression.Method;

			if (callExpression.Object != null)
			{
				// Creating a job that is based on a scope variable. We should infer its
				// type and method based on its value, and not from the expression tree.

				var objectValue = GetExpressionValue(callExpression.Object);
				if (objectValue == null)
				{
					throw new InvalidOperationException("Expression object should be not null.");
				}

				// TODO: BREAKING: Consider using `callExpression.Object.Type` expression instead.
				type = objectValue.GetType();

				// If an expression tree is based on interface, we should use its own
				// MethodInfo instance, based on the same method name and parameter types.
				method = type.GetNonOpenMatchingMethod(
					callExpression.Method.Name,
					callExpression.Method.GetParameters().Select(x => x.ParameterType).ToArray());
			}

			return new DelegateTask(type, method, GetExpressionValues(callExpression.Arguments));
		}

		private static object[] GetExpressionValues(IEnumerable<Expression> expressions)
		{
			return expressions.Select(GetExpressionValue).ToArray();
		}

		private static object GetExpressionValue(Expression expression)
		{
			return expression is ConstantExpression constantExpression
				? constantExpression.Value
				: CachedExpressionCompiler.Evaluate(expression);
		}
	}
}
