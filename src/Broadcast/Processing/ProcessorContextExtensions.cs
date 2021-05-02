using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Storage;

namespace Broadcast.Processing
{
	/// <summary>
	/// Extensions for the <see cref="IProcessorContext"/>
	/// </summary>
	public static class ProcessorContextExtensions
	{
		/// <summary>
		/// Set the state of the <see cref="ITask"/>.
		/// Propagates the state and the change event to the storage
		/// </summary>
		/// <param name="context"></param>
		/// <param name="task"></param>
		/// <param name="state"></param>
		public static void SetState(this IProcessorContext context, ITask task, TaskState state)
		{
			task.State = state;

			context.Store.Storage(s =>
			{
				var values = new Dictionary<string, object>
				{
					{"State", state},
					{$"{state}Change", DateTime.Now}
				};
				s.SetValues(new StorageKey($"tasks:values:{task.Id}"), values);
			});
		}

		/// <summary>
		/// Set a property and it's value to the storage associated to the <see cref="ITask"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <param name="task"></param>
		/// <param name="property"></param>
		/// <param name="value"></param>
		public static void SetValue<T>(this IProcessorContext context, ITask task, string property, T value)
		{
			context.Store.Storage(s =>
			{
				var values = new Dictionary<string, object>
				{
					{property, value}
				};
				s.SetValues(new StorageKey($"tasks:values:{task.Id}"), values);
			});
		}
	}
}
