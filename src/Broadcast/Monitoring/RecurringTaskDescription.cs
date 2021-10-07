using System;

namespace Broadcast.Monitoring
{
	/// <summary>
	/// Describes a recurring task with its new execution time
	/// </summary>
	public class RecurringTaskDescription
	{
		/// <summary>
		/// Gets the Id of the referenced RecurringTask that is executed next
		/// </summary>
		public string ReferenceId { get; set; }

		/// <summary>
		/// Gets the name of the RecurringTask
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the time of the next execution
		/// </summary>
		public DateTime NextExecution { get; set; }

		/// <summary>
		/// Gets the <see cref="TimeSpan"/> that defines the interval at which the task is executed
		/// </summary>
		public double? Interval { get; set; }
	}
}
