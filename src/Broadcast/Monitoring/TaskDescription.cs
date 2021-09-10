using System;
using Broadcast.EventSourcing;

namespace Broadcast.Monitoring
{
	/// <summary>
	/// Descibes the inner values of a task for monitoring
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
		/// Gets the milliseconds for scheduling at
		/// </summary>
		public double? Time { get; set; }

		/// <summary>
		/// Gets the name of the server that executed the task
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// Durationtime of the execution
		/// </summary>
		public double? Duration { get; set; }

		/// <summary>
		/// Starttime of the execution
		/// </summary>
		public DateTime? Start { get; set; }
	}
}
