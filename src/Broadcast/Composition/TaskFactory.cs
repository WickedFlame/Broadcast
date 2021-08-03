using Broadcast.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Broadcast.Composition
{
	/// <summary>
	/// Factory class that creates BroadcastTasks based on the delegate
	/// </summary>
	public static class TaskFactory
	{
		public static ITask CreateTask(Expression<Action> expression)
			=> CreateTask((LambdaExpression)expression);

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

			return new ExpressionTask(type, method, GetExpressionValues(callExpression.Arguments))
			{
				Name = $"{type.ToGenericTypeString()}.{method.Name}",
				State = TaskState.New
			};
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
