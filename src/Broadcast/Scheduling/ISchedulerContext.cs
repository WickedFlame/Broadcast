using System;
using Broadcast.Server;

namespace Broadcast.Scheduling
{
	/// <summary>
	/// Context object that is passed to the <see cref="IBackgroundServerProcess{T}"/> of the <see cref="Scheduler"/>
	/// </summary>
	public interface ISchedulerContext : IServerContext
	{
		/// <summary>
		/// Get a boolien indicating if the <see cref="IScheduler"/> is running
		/// </summary>
		bool IsRunning { get; set; }

		/// <summary>
		/// Gets the elapsed time since the Scheduler has been started
		/// </summary>
		TimeSpan Elapsed { get; }
	}
}
