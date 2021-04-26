using Broadcast.EventSourcing;
using Broadcast.Processing;
using System;
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
			: this(store, new TaskProcessor(Options.Default), new Scheduler(), Options.Default)
		{
		}

		public Broadcaster(ITaskStore store, ITaskProcessor processor, IScheduler scheduler)
			: this(store, processor, scheduler, Options.Default)
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

			store.RegisterDispatchers(new IDispatcher[]
			{
				new RecurringTaskDispatcher(this, store),
				new ScheduleTaskDispatcher(this),
				new ProcessTaskDispatcher(this)
			});

			_options = options;
		}

        /// <summary>
        /// The scheduler for timing tasks
        /// </summary>
        public IScheduler Scheduler{ get; }

        public ITaskProcessor Processor { get; }

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
		/// Wait for all threads to end
		/// </summary>
        public void WaitAll()
        {
	        Processor.WaitAll();
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
			Processor.AddHandler(target);
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
			_logger.Write($"Disposed Broadcaster {_options.ServerName}:{_id}");
		}
    }
}
