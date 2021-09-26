using Broadcast.EventSourcing;
using Broadcast.Processing;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Broadcast.Configuration;
using Broadcast.Diagnostics;
using Broadcast.Scheduling;
using Broadcast.Server;
using Task = System.Threading.Tasks.Task;

namespace Broadcast
{
	/// <summary>
	/// 
	/// </summary>
    public class Broadcaster : IBroadcaster
    {
	    private readonly Options _options;
	    private readonly ILogger _logger;
	    private readonly string _id = Guid.NewGuid().ToString();
	    private readonly BroadcasterConterxt _context;
	    private readonly BackgroundServerProcess<IBroadcasterConterxt> _server;

	    /// <summary>
		/// Creates a new Broadcaster
		/// </summary>
        public Broadcaster() : this(TaskStore.Default)
        {
        }

		/// <summary>
		/// Creates a new Broadcaster
		/// </summary>
		/// <param name="store"></param>
		public Broadcaster(ITaskStore store)
			: this(store, new Options())
		{
		}

		/// <summary>
		/// Creates a new Broadcaster
		/// </summary>
		/// <param name="store"></param>
		/// <param name="options"></param>
		public Broadcaster(ITaskStore store, Options options)
			: this(store, new TaskProcessor(store, options), new Scheduler(), options)
		{
		}

		/// <summary>
		/// Creates a new Broadcaster
		/// </summary>
		/// <param name="store"></param>
		/// <param name="processor"></param>
		/// <param name="scheduler"></param>
		public Broadcaster(ITaskStore store, ITaskProcessor processor, IScheduler scheduler)
			: this(store, processor, scheduler, new Options())
		{
		}

		/// <summary>
		/// Creates a new Broadcaster
		/// </summary>
		/// <param name="store"></param>
		/// <param name="processor"></param>
		/// <param name="scheduler"></param>
		/// <param name="options"></param>
		public Broadcaster(ITaskStore store, ITaskProcessor processor, IScheduler scheduler, Options options)
		{
			_logger = LoggerFactory.Create();
			_logger.Write($"Starting new Broadcaster {options.ServerName}:{_id}");

			Processor = processor;
			Scheduler = scheduler;
			Store = store;
			_options = options;

			store.RegisterDispatchers(_id, new IDispatcher[]
			{
				new RecurringTaskDispatcher(this, store),
				new ScheduleTaskDispatcher(this, store),
				new ProcessTaskDispatcher(this)
			});

			_context = new BroadcasterConterxt
			{
				Id = _id,
				IsRunning = true
			};
			_server = new BackgroundServerProcess<IBroadcasterConterxt>(_context);
			_server.StartNew(new BroadcasterHeartbeatDispatcher(store, _options));
		}

        /// <summary>
        /// The scheduler for timing tasks
        /// </summary>
        public IScheduler Scheduler{ get; }

		/// <summary>
		/// Gets the <see cref="ITaskProcessor"/>
		/// </summary>
        public ITaskProcessor Processor { get; }

		/// <summary>
		/// Gets the <see cref="ITaskStore"/>
		/// </summary>
		public ITaskStore Store { get; }

        /// <summary>
        /// Process the task
        /// </summary>
        /// <param name="task"></param>
        public void Process(ITask task)
        {
	        Processor.Process(task);
        }

        /// <summary>
		/// Wait for all tasks to be processed and all threads end
		/// </summary>
        public void WaitAll()
        {
	        Store.WaitAll();
	        Processor.WaitAll();
		}
		
		/// <summary>
		/// Dispose the Broadcaster
		/// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

		/// <summary>
		/// Dispose the Broadcaster
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

			Scheduler.Dispose();
			Processor.Dispose();
			Store.UnregisterDispatchers(_id);

			_context.IsRunning = false;

			_logger.Write($"Disposed Broadcaster {_options.ServerName}:{_id}");
		}
    }
}
