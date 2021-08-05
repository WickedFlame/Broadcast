﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// The Task that is enqueued
	/// </summary>
	public interface ITask
	{
		/// <summary>
		/// Id of the Task
		/// This is regenerated for each new Taskenqueu. Recurring tasks are cloned and enqueued multiple times
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the current <see cref="TaskState"/> of the task
		/// </summary>
		TaskState State { get; set; }

		/// <summary>
		/// Gets the times when the state of the task is changed
		/// </summary>
		IDictionary<TaskState, DateTime> StateChanges { get; }

		/// <summary>
		/// Gets the time for scheduling
		/// </summary>
		TimeSpan? Time { get; set; }

		/// <summary>
		/// Gets if a task is recurring
		/// </summary>
		bool IsRecurring { get; set; }

		/// <summary>
		/// The name of the Task.
		/// If none is provided by the Client a GUID is generated.
		/// Recurring Tasks keep the name for each recurrence contrary to the Id
		/// </summary>
		string Name { get; set; }
		
		/// <summary>
		/// Invoke the task
		/// </summary>
		/// <param name="invocation"></param>
		/// <returns></returns>
		object Invoke(TaskInvocation invocation);

		/// <summary>
		/// Clone the task for rescheduling
		/// </summary>
		/// <returns></returns>
		ITask Clone();
	}

	/// <summary>
	/// Represents a task that is stored in the <see cref="ITaskStore"/> and executed by the <see cref="BroadcastServer"/>
	/// </summary>
    public class BroadcastTask : ITask
    {
		private TaskState _state;

		public BroadcastTask()
		{
			StateChanges = new Dictionary<TaskState, DateTime>();
		}

		/// <summary>
		/// Creates an instance of <see cref="BroadcastTask"/>
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		public BroadcastTask(Type type, MethodInfo method, params object[] args)
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

			Validate(type, nameof(type), method, args.Length, nameof(args));

			Type = type;
			Method = method;
			Args = args;

		    StateChanges = new Dictionary<TaskState, DateTime>();
		    Id = Guid.NewGuid().ToString();
			Name = Guid.NewGuid().ToString();
		}

	    /// <inheritdoc/>
		public string Id { get; set; }

	    /// <inheritdoc/>
	    public TaskState State
	    {
		    get => _state;
		    set
		    {
			    _state = value;
			    StateChanges[value] = DateTime.Now;
		    }
	    }

	    /// <summary>
	    /// Gets the times when the state of the task is changed
	    /// </summary>
	    public IDictionary<TaskState, DateTime> StateChanges { get; set; }

		/// <inheritdoc/>
		public TimeSpan? Time { get; set; }

	    /// <inheritdoc/>
		public bool IsRecurring { get; set; }

		/// <inheritdoc/>
		public string Name { get; set; }

		/// <summary>
		/// Gets the type that the method to invoke is contained in
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// Gets the method that has to be invoked
		/// </summary>
		public MethodInfo Method { get; set; }

		/// <summary>
		/// Gets the arguments of the Method
		/// </summary>
		public IReadOnlyList<object> Args { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return Name;
		}

		/// <inheritdoc/>
		public object Invoke(TaskInvocation invocation)
		{
			return invocation.InvokeTask(this);
		}

		/// <inheritdoc/>
		public ITask Clone()
		{
			return new BroadcastTask(Type, Method, Args.ToArray())
			{
				State = TaskState.New,
				IsRecurring = IsRecurring,
				Time = Time,
				Name = Name
			};
		}

		protected static void Validate(Type type, string typeParameterName, MethodInfo method, int argumentCount, string argumentParameterName)
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
}
