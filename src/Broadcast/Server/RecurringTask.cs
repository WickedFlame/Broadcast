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
		/// Gets or sets the Id of the <see cref="ITask"/>
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the recurring <see cref="ITask"/>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the exact next executiontime for the recurring <see cref="ITask"/>
		/// </summary>
		public DateTime NextExecution { get; set; }
	}
}
