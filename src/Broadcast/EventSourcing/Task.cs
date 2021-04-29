﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Broadcast.Composition;

namespace Broadcast.EventSourcing
{
	public interface ITask
	{
		string Id { get; }

		TaskState State { get; set; }

		TimeSpan? Time { get; set; }

		bool IsRecurring { get; set; }

		/// <summary>
		/// The name of the Task.
		/// If none is provided by the Client a GUID is generated.
		/// Recurring Tasks keep the name for each recurrence contrary to the Id
		/// </summary>
		string Name { get; set; }

		object Invoke(TaskInvocation invocation);

		ITask Clone();
	}

	public interface ITask<T> : ITask
	{

	}

    public abstract class BroadcastTask : ITask
    {
	    protected BroadcastTask()
	    {
		    Id = Guid.NewGuid().ToString();
			Name = Guid.NewGuid().ToString();
		}

	    public string Id { get; }

		public TaskState State { get; set; }

		public TimeSpan? Time { get; set; }

		public bool IsRecurring { get; set; }

		/// <inheritdoc/>
		public string Name { get; set; }

		public abstract object Invoke(TaskInvocation invocation);

		public abstract ITask Clone();

		public override string ToString()
		{
			return Name;
		}

		protected static void Validate(Type type, string typeParameterName, MethodInfo method, string methodParameterName, int argumentCount, string argumentParameterName)
		{
			if (!method.IsPublic)
			{
				throw new NotSupportedException("Only public methods can be invoked in the background. Ensure your method has the `public` access modifier, and you aren't using explicit interface implementation.");
			}

			if (method.ContainsGenericParameters)
			{
				throw new NotSupportedException("Job method can not contain unassigned generic type parameters.");
			}

			if (method.DeclaringType == null)
			{
				throw new NotSupportedException("Global methods are not supported. Use class methods instead.");
			}

			if (!method.DeclaringType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
			{
				throw new ArgumentException($"The type `{method.DeclaringType}` must be derived from the `{type}` type.", typeParameterName);
			}

			if (method.ReturnType == typeof(void) && method.GetCustomAttribute<AsyncStateMachineAttribute>() != null)
			{
				throw new NotSupportedException("Async void methods are not supported. Use async Task instead.");
			}

			var parameters = method.GetParameters();

			if (parameters.Length != argumentCount)
			{
				throw new ArgumentException("Argument count must be equal to method parameter count.", argumentParameterName);
			}

			foreach (var parameter in parameters)
			{
				if (parameter.IsOut)
				{
					throw new NotSupportedException("Output parameters are not supported: there is no guarantee that specified method will be invoked inside the same process.");
				}

				if (parameter.ParameterType.IsByRef)
				{
					throw new NotSupportedException("Parameters, passed by reference, are not supported: there is no guarantee that specified method will be invoked inside the same process.");
				}

				var parameterTypeInfo = parameter.ParameterType.GetTypeInfo();

				if (parameterTypeInfo.IsSubclassOf(typeof(Delegate)) || parameterTypeInfo.IsSubclassOf(typeof(Expression)))
				{
					throw new NotSupportedException("Anonymous functions, delegates and lambda expressions aren't supported in job method parameters: it's very hard to serialize them and all their scope in general.");
				}
			}
		}
	}

    public class ActionTask : BroadcastTask
    {
		public Action Task { get; set; }

		public override object Invoke(TaskInvocation invocation)
		{
			Task.Invoke();

			return Id;
		}

		public override ITask Clone()
		{
			return new ActionTask
			{
				State = TaskState.New,
				Task = Task,
				IsRecurring = IsRecurring,
				Time = Time,
				Name = Name
			};
		}
	}

    public class DelegateTask<T> : BroadcastTask, ITask<T>
    {
		public Func<T> Task { get; set; }

		public override object Invoke(TaskInvocation invocation)
		{
			return Task.Invoke();
		}

		public override ITask Clone()
		{
			return new DelegateTask<T>
			{
				State = TaskState.New,
				Task = Task,
				IsRecurring = IsRecurring,
				Time = Time,
				Name = Name
			};
		}
	}

	public class ExpressionTask<T> : BroadcastTask, ITask<T>
    {
		public Expression<Func<T>> Task { get; set; }

		public override object Invoke(TaskInvocation invocation)
		{
			return Task.Compile().Invoke();
		}

		public override ITask Clone()
		{
			return new ExpressionTask<T>
			{
				State = TaskState.New,
				Task = Task,
				IsRecurring = IsRecurring,
				Time = Time,
				Name = Name
			};
		}
	}

    public class ExpressionTask : BroadcastTask
    {
	    public ExpressionTask(Type type, MethodInfo method, params object[] args)
	    {
		    if (type == null)
		    {
			    throw new ArgumentNullException(nameof(type));
		    }

		    if (method == null)
		    {
			    throw new ArgumentNullException(nameof(method));
		    }

		    if (args == null)
		    {
			    throw new ArgumentNullException(nameof(args));
		    }

		    Validate(type, nameof(type), method, nameof(method), args.Length, nameof(args));

		    Type = type;
		    Method = method;
		    Args = args;
	    }

	    public Type Type { get; }

	    public MethodInfo Method { get; }

	    public IReadOnlyList<object> Args { get; }

		public override string ToString()
	    {
		    return Name;
	    }

		public override object Invoke(TaskInvocation invocation)
		{
			return invocation.InvokeTask(this);
		}

		public override ITask Clone()
		{
			return new ExpressionTask(Type, Method, Args.ToArray())
			{
				State = TaskState.New,
				IsRecurring = IsRecurring,
				Time = Time,
				Name = Name
			};
		}
	}
}
