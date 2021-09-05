using System;
using Broadcast.EventSourcing;

namespace Broadcast.Monitoring
{
	/// <summary>
	/// Descibes a task
	/// </summary>
	public class TaskDescription
	{
		/// <summary>
		/// Gets the id of the <see cref="ITask"/>
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets the name of the <see cref="ITask"/>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets a value indicating if the <see cref="ITask"/> is recurring
		/// </summary>
		public bool IsRecurring { get; set; }

		/// <summary>
		/// Gets the <see cref="TaskState"/> of the <see cref="ITask"/>
		/// </summary>
		public TaskState State { get; set; }

		/// <summary>
		/// Gets the time for scheduling
		/// </summary>
		public TimeSpan? Time { get; set; }

		/// <summary>
		/// Gets the name of the server that executed the task
		/// </summary>
		public string Server { get; set; }
	}
}
