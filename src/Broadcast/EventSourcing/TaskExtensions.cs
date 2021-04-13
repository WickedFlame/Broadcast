
namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Extensions for tasks
	/// </summary>
	public static class TaskExtensions
	{
		/// <summary>
		/// Sets tha task to InProcess mode
		/// </summary>
		/// <param name="task"></param>
		public static void SetInprocess(this ITask task)
		{
			task.SetState(TaskState.InProcess);
		}

		/// <summary>
		/// Sets the task to Processed mode and removes it from the process queue
		/// </summary>
		/// <param name="task"></param>
		public static void SetProcessed(this ITask task)
		{
			task.SetState(TaskState.Processed);
		}

		/// <summary>
		/// Set the state of the task
		/// </summary>
		/// <param name="task"></param>
		/// <param name="state"></param>
		public static void SetState(this ITask task, TaskState state)
		{
			task.State = state;
		}
	}
}
