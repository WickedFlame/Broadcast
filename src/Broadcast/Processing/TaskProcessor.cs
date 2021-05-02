using System;
using Broadcast.Configuration;
using Broadcast.Diagnostics;
using Broadcast.EventSourcing;
using Broadcast.Server;

namespace Broadcast.Processing
{
    /// <summary>
    /// A class that can process different kinds of delegates and notifications
    /// </summary>
    public class TaskProcessor : ITaskProcessor, IDisposable
    {
	    private readonly object _processorLock = new object();
	    private readonly DispatcherLock _dispatcherLock;
		private readonly IBackgroundServerProcess<IProcessorContext> _server;
		private readonly ITaskQueue _queue;
		private readonly IProcessorContext _context;
		private readonly ILogger _logger;

		/// <summary>
		/// Creates a new TaskProcessor
		/// </summary>
		/// <param name="store"></param>
		/// <param name="options"></param>
		public TaskProcessor(ITaskStore store, Options options)
		{
			if (store == null)
			{
				throw new ArgumentNullException(nameof(store));
			}

			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			_logger = LoggerFactory.Create();
			_logger.Write($"Starting new TaskProcessor for {options.ServerName}");

			_context = new ProcessorContext(store, options);
			_queue = new TaskQueue();
			_dispatcherLock = new DispatcherLock();
			_server = new BackgroundServerProcess<IProcessorContext>(_context);
		}

		/// <summary>
		/// Gets the TaskQueue
		/// </summary>
		public ITaskQueue Queue => _queue;
		
		/// <summary>
		/// Wait for all threads in the taskprocessor to end
		/// </summary>
        public void WaitAll()
        {
	        _server.WaitAll();
        }

		/// <summary>
		/// Process the delegate task
		/// </summary>
		/// <param name="task">The task to process</param>
		public void Process(ITask task)
		{
			_logger.Write($"Enqueued task {task.Id}");

			_queue.Enqueue(task);
	        _context.SetState(task, TaskState.Queued);

			// check if a thread is allready processing the queue
			lock (_processorLock)
			{
				if (_dispatcherLock.IsLocked())
				{
					return;
				}

				// start new background thread to process all queued tasks
				_server.StartNew(new BackgroundTaskDispatcher(_dispatcherLock, _queue, _server));
			}
		}

		/// <summary>
		/// Dispose the TaskProcessor
		/// </summary>
        public void Dispose()
        {
	        _logger.Write($"Disposing TaskProcessor");
		}
    }
}
