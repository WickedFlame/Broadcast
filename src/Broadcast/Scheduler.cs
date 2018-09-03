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
        void Enqueue(Action task, TimeSpan time);

        void Dequeue(SchedulerTask task);

        TimeSpan Elapsed { get; }

        IEnumerable<SchedulerTask> Queue { get; }
    }

    public class Scheduler : IScheduler
    {
        private static int _schedulerCount;

        private object _lockHandle = new object();

        private readonly Thread _schedulerThread = null;
        private readonly Stopwatch _timer;

        private readonly List<SchedulerTask> _scheduleQueue = new List<SchedulerTask>();

        public Scheduler()
        {
            _timer = new Stopwatch();
            _timer.Start();

            _schedulerThread = new Thread(_ => Execute(this));
            if (!_schedulerThread.IsAlive)
            {
                _schedulerCount++;
                _schedulerThread.Name = $"Scheduler {_schedulerCount}";

                _schedulerThread.Start();
            }
        }

        public static int SchedulerCount => _schedulerCount;

        public TimeSpan Elapsed => _timer.Elapsed;

        public IEnumerable<SchedulerTask> Queue => _scheduleQueue.ToList();

        public void Enqueue(Action task, TimeSpan time)
        {
            lock (_lockHandle)
            {
                _scheduleQueue.Add(new SchedulerTask(task, Elapsed + time));
            }
        }

        public void Dequeue(SchedulerTask task)
        {
            lock (_lockHandle)
            {
                _scheduleQueue.Remove(task);
            }
        }

        private void Execute(IScheduler scheduler)
        {
            while (true)
            {
                // create a copy of the list
                IEnumerable<SchedulerTask> tasks = null;
                TimeSpan time;

                tasks = scheduler.Queue;
                time = scheduler.Elapsed;

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

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

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
                _schedulerThread.Abort();
                _schedulerCount--;
            }
        }
    }
}
