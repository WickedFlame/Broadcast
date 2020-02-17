using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast
{
    public interface IScheduler : IDisposable
    {
        /// <summary>
        /// Gets the elapsed time since the Scheduler has been started
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Gets the Queue of scheduled tasks
        /// </summary>
        IEnumerable<SchedulerTask> Queue { get; }

        /// <summary>
        /// Enqueues and schedules a new task
        /// </summary>
        /// <param name="task">The task to schedule</param>
        /// <param name="time">The time to execute the task at</param>
        void Enqueue(Action task, TimeSpan time);

        /// <summary>
        /// Removes the task from the schedule queue
        /// </summary>
        /// <param name="task">The task to remove</param>
        void Dequeue(SchedulerTask task);
    }

    public class Scheduler : IScheduler
    {
        private static int _schedulerCount;

        private object _lockHandle = new object();

        private readonly Thread _schedulerThread = null;
        private readonly Stopwatch _timer;
        private bool _isRunning = false;

        private readonly List<SchedulerTask> _scheduleQueue = new List<SchedulerTask>();

        public Scheduler()
        {
            _timer = new Stopwatch();
            _timer.Start();

            _isRunning = true;

            _schedulerThread = new Thread(_ => Execute(this));
            if (!_schedulerThread.IsAlive)
            {
                _schedulerCount++;
                _schedulerThread.Name = $"Scheduler {_schedulerCount}";

                _schedulerThread.Start();
            }
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
        public IEnumerable<SchedulerTask> Queue
        {
            get
            {
                lock (_lockHandle)
                {
                    return _scheduleQueue.ToList();
                }
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
            while (_isRunning)
            {
                // create a copy of the list
                IEnumerable<SchedulerTask> tasks = scheduler.Queue;
                var time = scheduler.Elapsed;
                

                foreach (var task in tasks)
                {
                    if (time > task.Time)
                    {
                        // execute task
                        task.Task.Invoke();

                        // remove task
                        scheduler.Dequeue(task);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (_schedulerThread != null)
            {
                //_schedulerThread.Abort();
                _isRunning = false;
                _schedulerCount--;
            }
        }
    }
}
