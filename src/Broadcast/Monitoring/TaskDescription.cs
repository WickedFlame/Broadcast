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

		/// <summary>
		/// Gets if the <see cref="ITask"/> is in a queued state. This is only true when a task is added new to the storage and before the <see cref="TaskStoreDispatcher"/> assigns a Server and moves the task to the queue.
		/// </summary>
		public bool Queued { get; set; }

		/// <summary>
		/// Gets if the <see cref="ITask"/> is allready assigned to a server and is added to the queue for further processing
		/// </summary>
		public bool Fetched { get; set; }
	}
}
