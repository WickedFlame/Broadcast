using System;
using Broadcast.EventSourcing;

namespace Broadcast
{
	/// <summary>
	/// Dispatcher that dispatches all added tasks to be processed by a taskprocessor
	/// </summary>
	public interface IDispatcher : IDisposable
	{
		/// <summary>
		/// Execute the Dispatcher to processes the task
		/// </summary>
		/// <param name="task"></param>
		void Execute(ITask task);
	}
}
