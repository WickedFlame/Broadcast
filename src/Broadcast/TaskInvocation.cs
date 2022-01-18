using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast
{
	public class TaskInvocation
	{
		private readonly IActivationContext _activationContext;

		public TaskInvocation(IActivationContext activationContext)
		{
			_activationContext = activationContext;
		}

		public object InvokeTask(BroadcastTask task)
		{
			object instance = null;
			if(!task.Method.IsStatic)
			{
				instance = _activationContext.Resolve(task.Type);
				if (instance == null)
				{
					throw new InvalidOperationException($"ActivationContext returned NULL instance of the '{task.Type}' type.");
				}
			}

			var arguments = BuildArguments(task);
			return InvokeMethod(task, instance, arguments);
		}

		private object[] BuildArguments(BroadcastTask task)
		{
			if (task == null)
			{
				return null;
			}

			var parameters = task.Method.GetParameters();
			var result = new List<object>(task.Args.Count);

			for (var i = 0; i < parameters.Length; i++)
			{
				//TODO: Refactor this
				//var parameter = parameters[i];
				var argument = task.Args[i];

				//TODO: Refactor this
				//var value = Substitutions.ContainsKey(parameter.ParameterType)
				//	? Substitutions[parameter.ParameterType](context)
				//	: argument;
				var value = argument;

				result.Add(value);
			}

			return result.ToArray();
		}

		private object InvokeMethod(BroadcastTask task, object instance, object[] arguments)
		{
			if (task == null)
			{
				return null;
			}

			try
			{
				var methodInfo = task.Method;
				var tuple = Tuple.Create(methodInfo, instance, arguments);
				//var returnType = methodInfo.ReturnType;

				//if (returnType.IsTaskLike(out var getTaskFunc))
				//{
				//		return InvokeOnTaskScheduler(tuple);
				//}

				return InvokeSynchronously(tuple);
			}
			catch (Exception ex)
			{
				//TODO: Log here
				System.Diagnostics.Trace.WriteLine(ex);
				throw;
			}
		}

		private object InvokeOnTaskScheduler(Tuple<MethodInfo, object, object[]> tuple)
		{
			//TODO: Create own TaskScheduler and store in options
			var _taskScheduler = TaskScheduler.Default;

			var scheduledTask = Task.Factory.StartNew(
				InvokeSynchronously,
				tuple,
				CancellationToken.None,
				TaskCreationOptions.None,
				_taskScheduler);

			var result = scheduledTask.GetAwaiter().GetResult();
			return result;
		}

		private static object InvokeSynchronously(object state)
		{
			var data = (Tuple<MethodInfo, object, object[]>)state;
			return data.Item1.Invoke(data.Item2, data.Item3);
		}
	}
}
