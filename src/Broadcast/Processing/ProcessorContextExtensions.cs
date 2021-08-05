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
			// setting the state also adds the timestamp and the state to the statechanges dictionary
			task.State = state;
			
			context.Store.Storage(s =>
			{
				var values = new DataObject
				{
					{"State", state},
					{$"{state}At", DateTime.Now}
				};
				s.SetValues(new StorageKey($"tasks:values:{task.Id}"), values);

				s.Set(new StorageKey($"task:{task.Id}"), task);
			});
		}

		/// <summary>
		/// Set a property and it's value to the storage associated to the <see cref="ITask"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <param name="task"></param>
		/// <param name="values"></param>
		public static void SetValues(this IProcessorContext context, ITask task, DataObject values)
		{
			context.Store.Storage(s =>
			{
				s.SetValues(new StorageKey($"tasks:values:{task.Id}"), values);
			});
		}
	}
}
