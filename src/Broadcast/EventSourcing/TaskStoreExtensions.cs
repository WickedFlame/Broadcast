using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.EventSourcing
{
	public static class TaskStoreExtensions
	{
		/// <summary>
		/// Sets tha task to InProcess mode
		/// </summary>
		/// <param name="store"></param>
		/// <param name="task"></param>
		public static void SetInprocess(this ITaskStore store, ITask task)
		{
			store.SetState(task, TaskState.InProcess);
		}

		/// <summary>
		/// Sets the task to Processed mode and removes it from the process queue
		/// </summary>
		/// <param name="store"></param>
		/// <param name="task"></param>
		public static void SetProcessed(this ITaskStore store, ITask task)
		{
			task.CloseTask();
			store.SetState(task, TaskState.Processed);
		}
	}
}
