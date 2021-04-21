using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast
{
	/// <summary>
	/// The scheduler for scheduled tasks
	/// </summary>
    public class Scheduler : IScheduler
    {
        private static int _schedulerCount;

        private readonly object _lockHandle = new object();

        private readonly Stopwatch _timer;
        private readonly CancellationToken _token = new CancellationToken();
		private bool _isRunning = false;

        private readonly List<SchedulerTask> _scheduleQueue = new List<SchedulerTask>();

		/// <summary>
		/// Creates a new scheduler for the broadcaster
		/// </summary>
        public Scheduler()
        {
	        _timer = new Stopwatch();
            _timer.Start();

            _isRunning = true;

            var schedulerTask = Task.Factory.StartNew(() => Execute(this), _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            //schedulerTask.ContinueWith(t =>
            //{
	           // _schedulerCount--;
            //});
            _schedulerCount++;
		}

        /// <summary>
        /// Gets the total count of Schedulers that are alive
        /// </summary>
        public static int SchedulerCount => _schedulerCount;

        /// <summary>
        /// Gets the elapsed time since the Scheduler has been started
        /// </summary>
        public TimeSpan Elapsed => _timer.Elapsed;

        /// <summary>
        /// Gets the Queue of scheduled tasks
        /// </summary>
        public IEnumerable<SchedulerTask> GetActiveTasks()
		{
			lock (_lockHandle)
			{
				return _scheduleQueue.ToList();
			}
		}

        /// <summary>
        /// Enqueues and schedules a new task
        /// </summary>
        /// <param name="task">The task to schedule</param>
        /// <param name="time">The time to execute the task at</param>
        public void Enqueue(Action task, TimeSpan time)
        {
            lock (_lockHandle)
            {
                _scheduleQueue.Add(new SchedulerTask(task, Elapsed + time));
            }
        }

        /// <summary>
        /// Removes the task from the schedule queue
        /// </summary>
        /// <param name="task">The task to remove</param>
        public void Dequeue(SchedulerTask task)
        {
            lock (_lockHandle)
            {
                _scheduleQueue.Remove(task);
            }
        }

        private void Execute(IScheduler scheduler)
        {
            while (_isRunning && !_token.IsCancellationRequested)
            {
                var time = scheduler.Elapsed;

                foreach (var task in scheduler.GetActiveTasks())
                {
                    if (time > task.Time)
                    {
	                    // remove task
	                    scheduler.Dequeue(task);

						// execute task
						task.Task.Invoke();
                    }
                }
            }
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

            _isRunning = false;
            _schedulerCount--;
		}
    }
}
