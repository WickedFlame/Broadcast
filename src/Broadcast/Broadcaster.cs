using Broadcast.EventSourcing;
using System;
using System.Linq.Expressions;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace Broadcast
{
    public class Broadcaster : IBroadcaster, IScheduledBroadcast
    {
        private IProcessorContext _context;
        private IScheduler _scheduler;

        public Broadcaster()
        {
        }

        public Broadcaster(ProcessorMode mode)
        {
            Context.Mode = mode;
        }

        public Broadcaster(ITaskStore store)
        {
            Context.Store = store;
        }

        public Broadcaster(ProcessorMode mode, ITaskStore store)
        {
            Context.Mode = mode;
            Context.Store = store;
        }

        /// <summary>
        /// Gets the ProcessorContext that containes all information create a TaskProcessor
        /// </summary>
        public IProcessorContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = ProcessorContextFactory.GetContext();
                }

                return _context;
            }
        }

        /// <summary>
        /// The scheduler for timing tasks
        /// </summary>
        public IScheduler Scheduler
        {
            get
            {
                if (_scheduler == null)
                {
                    _scheduler = new Scheduler();
                }

                return _scheduler;
            }
        }

        /// <summary>
        /// Send a delegate to the task processor
        /// </summary>
        /// <param name="action">The Task to process</param>
        public void Send(Action action)
        {
            var task = TaskFactory.CreateTask(action);
            using (var processor = Context.Open())
            {
                processor.Process(task);
            }
        }

        /// <summary>
        /// Processes a task Asynchronously. ContextMode has to be Default for Async Processing
        /// </summary>
        /// <param name="action">The Task to process</param>
        /// <returns>Task thread</returns>
        public async Task SendAsync(Action action)
        {
            EnsureContextModeForAsync();

            var task = TaskFactory.CreateTask(action);
            using (var processor = Context.Open())
            {
                await Task.Run(() => processor.Process(task));
            }
        }

        /// <summary>
        /// Sends a INotification to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        public void Send<T>(Func<T> notification) where T : INotification
        {
            var task = TaskFactory.CreateNotifiableTask(notification);
            using (var processor = Context.Open())
            {
                processor.Process(task);
            }
        }

        /// <summary>
        /// Sends a INotification async to the processor. The INotification will be passed to all registered Handlers of the same type. Async method calls are only allowed in ProcessorMode.Default
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <returns>Task thread</returns>
        public Task SendAsync<T>(Func<T> notification) where T : INotification
        {
            EnsureContextModeForAsync();

            var task = TaskFactory.CreateNotifiableTask(notification);
            using (var processor = Context.Open())
            {
                return Task.Run(() => processor.Process(task));
            }
        }

        /// <summary>
        /// Processes a task async
        /// </summary>
        /// <typeparam name="T">The return type of the process</typeparam>
        /// <param name="process">The function to execute</param>
        /// <returns>The value of the function</returns>
        public System.Threading.Tasks.Task<T> ProcessAsync<T>(Func<T> process)
        {
            EnsureContextModeForAsync();

            var task = TaskFactory.CreateTask(process);
            using (var processor = Context.Open())
            {
                return Task.Factory.StartNew(() => processor.ProcessUnhandled(task));
            }
        }

        /// <summary>
        /// Register a INotificationTarget that gets called when a INotification of the same type is sent
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="target">The INotificationTarget that handles the INotification</param>
        public void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification
        {
            RegisterHandler<T>(a => target.Handle(a));
        }

        /// <summary>
        /// Register a delegate that gets called when a INotification of the same type is sent
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="target">The delegate that handles the INotification</param>
        public void RegisterHandler<T>(Action<T> target) where T : INotification
        {
            using (var processor = Context.Open())
            {
                processor.AddHandler(target);
            }
        }

        /// <summary>
        /// Schedules a new task. The task will be executed at the time passed
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="time">The time to execute the task at</param>
        public void Schedule(Action task, TimeSpan time)
        {
            Scheduler.Enqueue(() => Send(task), time);
        }

        /// <summary>
        /// Creates and schedules a new task that will recurr at the given interval
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="time">The interval time to execute the task at</param>
        public void Recurring(Action task, TimeSpan time)
        {
            Scheduler.Enqueue(() =>
            {
                // execute the task
                Send(task);

                // reschedule the task
                Recurring(task, time);
            }, time);
        }

        /// <summary>
        /// Schedules a INotification that is sent to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="task">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <param name="time">The interval time to execute the task at</param>
        public void Schedule<T>(Func<T> task, TimeSpan time) where T : INotification
        {
            //Scheduler.Enqueue(notification, time);
            Scheduler.Enqueue(() => Send(task), time);
        }

        /// <summary>
        /// Schedules a recurring INotification task that is sent to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <param name="time">The interval time to execute the task at</param>
        public void Recurring<T>(Func<T> notification, TimeSpan time) where T : INotification
        {
            Scheduler.Enqueue(() =>
            {
                // execute the task
                Send(notification);

                // reschedule the task
                Recurring(notification, time);
            }, time);
        }







        private void EnsureContextModeForAsync()
        {
            if (Context.Mode == ProcessorMode.Default)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Async message handling is only alowed when running the broadcast context in Default Mode.");
            sb.AppendLine("Use Send(message) if ProcessorMode is intentialy run in another mode than ProcessorMode.Default");

            throw new InvalidOperationException(sb.ToString());
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

            if (_scheduler != null)
            {
                _scheduler.Dispose();
                _scheduler = null;
            }
        }
    }
}
