using Broadcast.EventSourcing;
using Broadcast.Processing;
using System;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Broadcast.Configuration;
using Task = System.Threading.Tasks.Task;

namespace Broadcast
{
    public class Broadcaster : IBroadcaster
    {
		static IBroadcaster _server;

		static Broadcaster()
		{
			// setup the default server
			// this is created even if it is not used
			Setup(s => { });
		}

		public static IBroadcaster Server => _server;

		public static void Setup(Action<IBroadcaster> setup)
		{
			var server = new Broadcaster();
			setup(server);
			_server = server;
		}

		private IProcessorContext _context;
        private IScheduler _scheduler;
        private Options _options;
		
        public Broadcaster() : this(Options.Default)
        {
        }

        public Broadcaster(Options options)
        {
			_options = options;
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
                    _context = new ProcessorContext();
                }

                return _context;
			}
			set => _context = value;
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
	        //Context.Store.Add(task);
			using (var processor = Context.Open())
			{
				processor.Process(task);
			}
		}

        public void WaitAll()
        {
	        using (var processor = Context.Open())
	        {
		        processor.WaitAll();
	        }
		}

        public ITaskStore GetStore()
        {
	        return Context.Store;
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
