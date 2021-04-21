﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;

namespace Broadcast
{
	/// <summary>
	/// Extensions for Broadcaster
	/// </summary>
	public static class BroadcasterExtensions
	{
		/// <summary>
		/// Send a delegate to the task processor
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="action">The Task to process</param>
		public static void Send(this IBroadcaster broadcaster, Expression<Action> action)
		{
			var task = TaskFactory.CreateTask(action);

			broadcaster.GetStore().Add(task);
		}

		/// <summary>
		/// Sends a INotification to the processor. The INotification will be passed to all registered Handlers of the same type
		/// </summary>
		/// <typeparam name="T">The notification type</typeparam>
		/// <param name="broadcaster"></param>
		/// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
		public static void Send<T>(this IBroadcaster broadcaster, Expression<Func<T>> notification) where T : INotification
		{
			var task = TaskFactory.CreateNotifiableTask(notification);

			broadcaster.GetStore().Add(task);
		}

		///// <summary>
		///// Schedules a task
		///// </summary>
		///// <param name="broadcaster"></param>
		///// <param name="task"></param>
		///// <param name="time"></param>
		//public static void Schedule(this IBroadcaster broadcaster, ITask task, TimeSpan time)
		//{
		//	broadcaster.GetStore().Add(task);
		//	broadcaster.Scheduler.Enqueue(() => broadcaster.Process(task), time);
		//}

		///// <summary>
		///// Create a recurring task
		///// </summary>
		///// <param name="broadcaster"></param>
		///// <param name="task"></param>
		///// <param name="time"></param>
		//public static void Recurring(this IBroadcaster broadcaster, ITask task, TimeSpan time)
		//{
		//	broadcaster.GetStore().Add(task);
		//	broadcaster.Scheduler.Enqueue(() =>
		//	{
		//		// execute the task
		//		broadcaster.Process(task);

		//		// reschedule the task
		//		broadcaster.Recurring(task.Clone(), time);
		//	}, time);
		//}




		/// <summary>
		/// Schedules a new task. The task will be executed at the time passed
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The time to execute the task at</param>
		public static void Schedule(this IBroadcaster broadcaster, Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;

			broadcaster.GetStore().Add(task);
		}

		/// <summary>
		/// Schedules a INotification that is sent to the processor. The INotification will be passed to all registered Handlers of the same type
		/// </summary>
		/// <typeparam name="T">The notification type</typeparam>
		/// <param name="expression">The delegate returning the notification that will be processed and passed to the handlers</param>
		/// <param name="time">The interval time to execute the task at</param>
		public static void Schedule<T>(this IBroadcaster broadcaster, Expression<Func<T>> expression, TimeSpan time) where T : INotification
		{
			var task = TaskFactory.CreateNotifiableTask(expression);
			task.Time = time;

			broadcaster.GetStore().Add(task);
		}


		/// <summary>
		/// Create a recurring task
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The interval time to execute the task at</param>
		public static void Recurring(this IBroadcaster broadcaster, Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;
			task.IsRecurring = true;

			broadcaster.GetStore().Add(task);
		}
		
		/// <summary>
		/// Schedules a recurring INotification task that is sent to the processor. The INotification will be passed to all registered Handlers of the same type
		/// </summary>
		/// <typeparam name="T">The notification type</typeparam>
		/// <param name="broadcaster"></param>
		/// <param name="expression">The delegate returning the notification that will be processed and passed to the handlers</param>
		/// <param name="time">The interval time to execute the task at</param>
		public static void Recurring<T>(this IBroadcaster broadcaster, Expression<Func<T>> expression, TimeSpan time) where T : INotification
		{
			var task = TaskFactory.CreateNotifiableTask(expression);
			task.Time = time;
			task.IsRecurring = true;

			broadcaster.GetStore().Add(task);
		}
	}
}
