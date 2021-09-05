using System;
using Broadcast.EventSourcing;

namespace Broadcast.Server
{
	/// <summary>
	/// Storage model for recurring tasks
	/// </summary>
	public class RecurringTask
	{
		/// <summary>
		/// Gets or sets the Id of the referenced <see cref="ITask"/> that will be executed next
		/// </summary>
		public string ReferenceId { get; set; }

		/// <summary>
		/// Gets or sets the name of the recurring <see cref="ITask"/>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the exact next executiontime for the recurring <see cref="ITask"/>
		/// </summary>
		public DateTime NextExecution { get; set; }

		/// <summary>
		/// Gets the <see cref="TimeSpan"/> that defines the interval at which the task is executed
		/// </summary>
		public TimeSpan? Interval { get; set; }
	}
}
