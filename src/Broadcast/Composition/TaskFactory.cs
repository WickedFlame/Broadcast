using System;
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
		/// Creates a task from an action
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		public static ITask CreateTask(Action task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			return new ActionTask
			{
				Task = task,
				State = TaskState.New
			};
		}

		/// <summary>
		/// Creates a task from a func
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <returns></returns>
		public static ITask CreateTask<T>(Func<T> task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			return new DelegateTask<T>
			{
				Task = task,
				State = TaskState.New
			};
		}

		/// <summary>
		/// Creates a Notification Task based on the Func delegate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <returns></returns>
		public static ExpressionTask<T> CreateNotifiableTask<T>(Expression<Func<T>> task) where T : INotification
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			return new ExpressionTask<T>
			{
				Task = task,
				State = TaskState.New
			};
		}

		/// <summary>
		/// Create a task from a labdaexpression
		/// </summary>
		/// <param name="methodCall"></param>
		/// <returns></returns>
		public static ITask CreateTask(LambdaExpression methodCall)
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

			return new ExpressionTask(type, method, GetExpressionValues(callExpression.Arguments));
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
