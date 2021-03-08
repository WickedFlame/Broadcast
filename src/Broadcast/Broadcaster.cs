using Broadcast.EventSourcing;
using Broadcast.Processing;
using System;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Task = System.Threading.Tasks.Task;

namespace Broadcast
{
    public class Broadcaster : IBroadcaster, IScheduledBroadcast
    {
		static IBroadcaster _server;

		static Broadcaster()
		{
			// setup the default server
			// this is created even if it is not used
			_server = new Broadcaster();
		}

		public static IBroadcaster Server => _server;

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

        public void Process(ITask task)
        {
	        using (var processor = Context.Open())
	        {
		        processor.Process(task);
	        }
		}

  //      /// <summary>
  //      /// Send a delegate to the task processor
  //      /// </summary>
  //      /// <param name="action">The Task to process</param>
  //      public void Send(Expression<Action> action)
  //      {
  //          var task = TaskFactory.CreateTask(action);
  //          Process(task);
  //          //using (var processor = Context.Open())
  //          //{
  //          //    processor.Process(task);
  //          //}
  //      }
		
		///// <summary>
		///// Sends a INotification to the processor. The INotification will be passed to all registered Handlers of the same type
		///// </summary>
		///// <typeparam name="T">The notification type</typeparam>
		///// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
		//public void Send<T>(Expression<Func<T>> notification) where T : INotification
  //      {
  //          var task = TaskFactory.CreateNotifiableTask(notification);
  //          Process(task);
  //          //using (var processor = Context.Open())
  //          //{
  //          //    processor.Process(task);
  //          //}
  //      }

        ///// <summary>
        ///// Sends a INotification async to the processor. The INotification will be passed to all registered Handlers of the same type. Async method calls are only allowed in ProcessorMode.Default
        ///// </summary>
        ///// <typeparam name="T">The notification type</typeparam>
        ///// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        ///// <returns>Task thread</returns>
        //public Task SendAsync<T>(Expression<Func<T>> notification) where T : INotification
        //{
        //    var task = TaskFactory.CreateNotifiableTask(notification);
        //    using (var processor = new AsyncTaskProcessor(Context.Store, Context.NotificationHandlers))
        //    {
        //        return processor.ProcessAsync(task);
        //    }
        //}
		
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
        public void Schedule(Expression<Action> task, TimeSpan time)
        {
            Scheduler.Enqueue(() => this.Send(task), time);
        }

        /// <summary>
        /// Creates and schedules a new task that will recurr at the given interval
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="time">The interval time to execute the task at</param>
        public void Recurring(Expression<Action> task, TimeSpan time)
        {
            Scheduler.Enqueue(() =>
            {
				// execute the task
				this.Send(task);

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
        public void Schedule<T>(Expression<Func<T>> task, TimeSpan time) where T : INotification
        {
            Scheduler.Enqueue(() => this.Send(task), time);
        }

        /// <summary>
        /// Schedules a recurring INotification task that is sent to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <param name="time">The interval time to execute the task at</param>
        public void Recurring<T>(Expression<Func<T>> notification, TimeSpan time) where T : INotification
        {
            Scheduler.Enqueue(() =>
            {
				// execute the task
				this.Send(notification);

                // reschedule the task
                Recurring(notification, time);
            }, time);
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
