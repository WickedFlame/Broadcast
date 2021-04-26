using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Diagnostics;
using Broadcast.Server;

namespace Broadcast.Scheduling
{
	/// <summary>
	/// The scheduler for scheduled tasks
	/// </summary>
    public class Scheduler : IScheduler
    {
        private static int _schedulerCount;

		private readonly ISchedulerContext _context;
		private readonly IScheduleQueue _scheduleQueue;
		private readonly IBackgroundServerProcess<ISchedulerContext> _backgroundProcess;
		private readonly ILogger _logger;

		/// <summary>
		/// Creates a new <see cref="IScheduler"/> for the broadcaster
		/// </summary>
        public Scheduler() 
			: this (new ScheduleQueue())
        {
		}

		/// <summary>
		/// Creates a new <see cref="IScheduler"/> for the broadcaster
		/// </summary>
		/// <param name="queue"></param>
		public Scheduler(IScheduleQueue queue)
		{
			_logger = LoggerFactory.Create();

			_scheduleQueue = queue ?? throw new ArgumentNullException();
			_context = new SchedulerContext
			{
				IsRunning = true
			};

			_backgroundProcess = new BackgroundServerProcess<ISchedulerContext>(_context);
			_backgroundProcess.StartNew(new SchedulerTaskDispatcher(_scheduleQueue));

			_schedulerCount++;

			_logger.Write($"Started new Scheduler. New SchedulerCount: {_schedulerCount}");
		}

        /// <summary>
        /// Gets the total count of Schedulers that are alive
        /// </summary>
        public static int SchedulerCount => _schedulerCount;

        /// <summary>
        /// Enqueues and schedules a new task
        /// </summary>
        /// <param name="task">The task to schedule</param>
        /// <param name="time">The time to execute the task at</param>
        public void Enqueue(Action task, TimeSpan time)
        {
	        _scheduleQueue.Enqueue(new SchedulerTask(task, _context.Elapsed + time));
        }

		/// <summary>
		/// Gets a list of all active scheduled tasks
		/// </summary>
		/// <returns></returns>
        public IEnumerable<SchedulerTask> ScheduledTasks()
        {
	        return _scheduleQueue.ToList();
        }

		/// <summary>
		/// Dispose the Scheduler. This stops the scheduler thread associated with this scheduler
		/// </summary>
		public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

		/// <summary>
		/// Dispose the scheduler
		/// </summary>
		/// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _context.IsRunning = false;

            _backgroundProcess.WaitAll();
            _schedulerCount--;
            _logger.Write($"Disposed Scheduler. New SchedulerCount: {_schedulerCount}");
		}
    }
}
