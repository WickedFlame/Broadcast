using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast
{
    public interface IScheduler : IDisposable
    {
        void Enqueue(Action task, TimeSpan time);
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

            _schedulerThread = new Thread(_ => Execute(_scheduleQueue, _timer, _lockHandle));
            if (!_schedulerThread.IsAlive)
            {
                _schedulerCount++;
                _schedulerThread.Name = $"Scheduler {_schedulerCount}";

                _schedulerThread.Start();
            }
        }

        public static int SchedulerCount => _schedulerCount;

        public void Enqueue(Action task, TimeSpan time)
        {
            lock (_lockHandle)
            {
                _scheduleQueue.Add(new SchedulerTask(task, _timer.Elapsed + time));
            }
        }

        private void Execute(List<SchedulerTask> scheduleQueue, Stopwatch timer, object lockHandle)
        {
            while (true)
            {
                // create a copy of the list
                IEnumerable<SchedulerTask> tasks = null;
                TimeSpan time;
                lock (lockHandle)
                {
                    tasks = scheduleQueue.ToList();
                    time = timer.Elapsed;
                }

                foreach(var task in tasks)
                {
                    if(time > task.Time)
                    {
                        // execute task
                        task.Task();

                        // remove task
                        lock (lockHandle)
                        {
                            scheduleQueue.Remove(task);
                        }
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
