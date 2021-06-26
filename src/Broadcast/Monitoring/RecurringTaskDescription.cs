using System;

namespace Broadcast.Monitoring
{
	/// <summary>
	/// Describes a recurring task with its new execution time
	/// </summary>
	public class RecurringTaskDescription
	{
		/// <summary>
		/// Gets the name of the RecurringTask
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the time of the next execution
		/// </summary>
		public DateTime NextExecution { get; set; }
	}
}
