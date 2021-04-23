using System;
using System.Diagnostics;

namespace Broadcast.Scheduling
{
	/// <summary>
	/// Context object that is passed to the <see cref="IBackgroundServerProcess{T}"/> of the <see cref="Scheduler"/>
	/// </summary>
	public class SchedulerContext : ISchedulerContext
	{
		private readonly Stopwatch _timer;

		/// <summary>
		/// Creates a new instance of the SchedulerContext
		/// </summary>
		public SchedulerContext()
		{
			_timer = new Stopwatch();
			_timer.Start();
		}

		/// <summary>
		/// Get a boolien indicating if the <see cref="IScheduler"/> is running
		/// </summary>
		public bool IsRunning{ get; set; }

		/// <summary>
		/// Gets the elapsed time since the Scheduler has been started
		/// </summary>
		public TimeSpan Elapsed => _timer.Elapsed;
	}
}
