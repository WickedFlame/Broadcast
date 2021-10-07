using System;
using System.Threading;
using Broadcast.Server;

namespace Broadcast.Scheduling
{
	/// <summary>
	/// Dispatcher for the <see cref="IScheduler"/> that executes the scheduled <see cref="SchedulerTask"/>
	/// </summary>
	public class SchedulerBackgroundProcess : IBackgroundDispatcher<ISchedulerContext>
	{
		private readonly IScheduleQueue _queue;

		/// <summary>
		/// Cretes a new instance of the SchedulerTaskDispatcher
		/// </summary>
		/// <param name="queue"></param>
		public SchedulerBackgroundProcess(IScheduleQueue queue)
		{
			_queue = queue ?? throw new ArgumentNullException(nameof(queue));
		}

		/// <summary>
		/// Execute the Dispatcher to processes the scheduled tasks
		/// </summary>
		/// <param name="context"></param>
		public void Execute(ISchedulerContext context)
		{
			while (context.IsRunning)
			{
				var time = context.Elapsed;
				foreach (var task in _queue.ToList())
				{
					if (time > task.Time)
					{
						// remove task
						_queue.Dequeue(task);

						// execute task
						task.Task.Invoke(task.Id);
					}
				}

				// Delay the thread to avoid high CPU usage with the infinite loop
				System.Threading.Tasks.Task.Delay(50).Wait();
			}
		}
	}
}
