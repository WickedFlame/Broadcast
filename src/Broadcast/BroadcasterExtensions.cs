using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Server;

namespace Broadcast
{
	/// <summary>
	/// Extensions for Broadcaster
	/// </summary>
	public static class BroadcasterExtensions
	{
		/// <summary>
		/// Send a delegate to the task processor server
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="action"></param>
		public static void Execute(this IBroadcaster broadcaster, Expression<Action> action)
		{
			var task = TaskFactory.CreateTask(action);

			broadcaster.Store.Add(task);
		}

		/// <summary>
		/// Gets all Tasks that have been processed
		/// </summary>
		/// <param name="broadcaster"></param>
		public static IEnumerable<ITask> GetProcessedTasks(this IBroadcaster broadcaster)
		{
			return broadcaster.Store.Where(s => s.State == TaskState.Processed);
		}

		/// <summary>
		/// Send a delegate to the task processor server
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="action">The Task to process</param>
		public static string Send(this IBroadcaster broadcaster, Expression<Action> action)
		{
			var client = new BroadcastingClient(broadcaster.Store);
			return client.Send(action);
		}

		/// <summary>
		/// Schedules a new task. The task will be executed at the time that is given as parameter.
		/// Schedueld Tasks only get executed if the <see cref="Broadcaster"/> is still running. If a <see cref="Broadcaster"/> is disposed or closed, all scheduled tasks are lost.
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The time to execute the task at</param>
		public static string Schedule(this IBroadcaster broadcaster, Expression<Action> expression, TimeSpan time)
		{
			var client = new BroadcastingClient(broadcaster.Store);
			return client.Schedule(expression, time);
		}

		/// <summary>
		/// Create or update a recurring task.
		/// The name of the Recurring Task is generated based on the <see cref="Expression"/>.
		/// If multiple Recurring Tasks have the same signature, this could cause some confusion
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The interval time to execute the task at</param>
		public static void Recurring(this IBroadcaster broadcaster, Expression<Action> expression, TimeSpan time)
			=> Recurring(broadcaster, null, expression, time);

		/// <summary>
		/// Create or update a recurring task. 
		/// Recurring Tasks are referenced by the name. 
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="name">Unitque name of the recurring Task</param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The interval time to execute the task at</param>
		public static void Recurring(this IBroadcaster broadcaster, string name, Expression<Action> expression, TimeSpan time)
		{
			var client = new BroadcastingClient(broadcaster.Store);
			client.Recurring(name, expression, time);
		}

		/// <summary>
		/// Deltete a <see cref="ITask"/> from the Executionpipeline.
		/// If a task is allready in the state of <see cref="TaskState.Processing"/> the delete will be ignored
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="taskId"></param>
		public static void DeleteTask(this IBroadcaster broadcaster, string taskId)
		{
			var client = new BroadcastingClient(broadcaster.Store);
			client.DeleteTask(taskId);
		}

		/// <summary>
		/// Delete a recurring task with the associated task execution
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="name">The name of the recurring Task</param>
		public static void DeleteRecurringTask(this IBroadcaster broadcaster, string name)
		{
			var client = new BroadcastingClient(broadcaster.Store);
			client.DeleteRecurringTask(name);
		}
	}
}
