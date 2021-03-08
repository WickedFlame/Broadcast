using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;

namespace Broadcast
{
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
			broadcaster.Process(task);
			//using (var processor = Context.Open())
			//{
			//    processor.Process(task);
			//}
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
			broadcaster.Process(task);
			//using (var processor = Context.Open())
			//{
			//    processor.Process(task);
			//}
		}
	}
}
