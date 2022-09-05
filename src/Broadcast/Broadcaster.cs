using Broadcast.EventSourcing;
using Broadcast.Processing;
using System;
using Broadcast.Configuration;
using Broadcast.Diagnostics;
using Broadcast.Scheduling;
using Broadcast.Server;

namespace Broadcast
{
	/// <summary>
	/// 
	/// </summary>
    public class Broadcaster : IBroadcaster
    {
	    private readonly ProcessorOptions _options;
	    private readonly ILogger _logger;
	    private readonly string _id = Guid.NewGuid().ToString();
	    private readonly BroadcasterConterxt _context;
	    private readonly BackgroundServerProcess<IBroadcasterConterxt> _server;

	    /// <summary>
		/// Creates a new Broadcaster
		/// </summary>
        public Broadcaster() 
            : this(new TaskStore())
        {
        }

		/// <summary>
		/// Creates a new Broadcaster
		/// </summary>
		/// <param name="store"></param>
		public Broadcaster(ITaskStore store)
			: this(store, new ProcessorOptions())
		{
		}

		/// <summary>
		/// Creates a new Broadcaster
		/// </summary>
		/// <param name="store"></param>
		/// <param name="options"></param>
		public Broadcaster(ITaskStore store, ProcessorOptions options)
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
			: this(store, processor, scheduler, new ProcessorOptions())
		{
		}

		/// <summary>
		/// Creates a new Broadcaster
		/// </summary>
		/// <param name="store"></param>
		/// <param name="processor"></param>
		/// <param name="scheduler"></param>
		/// <param name="options"></param>
		public Broadcaster(ITaskStore store, ITaskProcessor processor, IScheduler scheduler, ProcessorOptions options)
		{
			_logger = LoggerFactory.Create();
			_logger.Write($"Starting new Broadcaster {options.ServerName}:{_id}");

            EventHandlers = new EventHandlerContext(options.ActivationContext);
			Processor = processor;
			Scheduler = scheduler;
			Store = store;
			_options = options;

			store.RegisterDispatchers(_id, new IDispatcher[]
			{
				new RecurringTaskDispatcher(this, store),
				new ScheduleTaskDispatcher(this, store),
				new ProcessTaskDispatcher(this, store)
			});

			_context = new BroadcasterConterxt
			{
				Id = _id
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

		public IEventHandlerContext EventHandlers { get; }

		/// <summary>
		/// Gets the name of the instance. This is equal to the <see cref="Options.ServerName"/>
		/// </summary>
		public string Name => _options.ServerName;

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

			// dispatching of tasks to the servers is done anync
			// if there are tasks in the storage that are not dispatched, they will not get dispatched at all
			// wait for the items in the store to be dispatched all
            Store.WaitAll();

			// unregister dispatcher so the server will not get any more tasks to process
			Store.UnregisterDispatchers(_id);
			
			Scheduler.Dispose();
			Processor.WaitAll();
			Processor.Dispose();

            _context.Stop();

			Store.RemoveServer(new ServerModel { Id = _id, Name = Name });

			_logger.Write($"Disposed Broadcaster {_options.ServerName}:{_id}");
		}
    }
}
