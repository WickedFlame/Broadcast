using Broadcast.Composition;
using System;

namespace Broadcast
{
	public class BackgroundTaskClient
	{
		// local jobs need a local server running

		public static void Recurring(Action expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Recurring(task, time);
		}

		public static void Schedule(Action expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Schedule(task, time);
		}

		public static void Send(Action expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}



		public static void Recurring<T>(Func<T> expression, TimeSpan time) where T : INotification
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Recurring(task, time);
		}

		public static void Schedule<T>(Func<T> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Schedule(task, time);
		}

		public static void Send<T>(Func<T> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}
	}
}
