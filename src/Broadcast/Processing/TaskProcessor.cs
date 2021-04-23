using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		private readonly BackgroundServerProcess<IProcessorContext> _server;
		private readonly ITaskQueue _queue;

		/// <summary>
		/// Creates a new TaskProcessor
		/// </summary>
		/// <param name="context"></param>
        public TaskProcessor(IProcessorContext context)
        {
            _queue = new TaskQueue();
            _dispatcherLock = new DispatcherLock();
            _server = new BackgroundServerProcess<IProcessorContext>(context);
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
	        _queue.Enqueue(task);
	        task.SetState(TaskState.Queued);

	        // check if a thread is allready processing the queue
			lock(_processorLock)
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
        }
    }
}
