using System;
using Broadcast.EventSourcing;
using Broadcast.Server;

namespace Broadcast.Processing
{
	/// <summary>
	/// BackgroundDispatcher that dispatchers enqueued tasks to <see cref="TaskExecutionDispatcher"/>
	/// Each task is run in its own thread
	/// </summary>
	public class BackgroundTaskDispatcher : IBackgroundDispatcher<IProcessorContext>
	{
		private readonly DispatcherLock _dispatcherLock;
		private readonly ITaskQueue _queue;
		private readonly IBackgroundServerProcess<IProcessorContext> _server;

		/// <summary>
		/// Creates a new instance of a BackgroundTaskDispatcher
		/// </summary>
		/// <param name="dispatcherLock"></param>
		/// <param name="queue"></param>
		/// <param name="server"></param>
		public BackgroundTaskDispatcher(DispatcherLock dispatcherLock, ITaskQueue queue, IBackgroundServerProcess<IProcessorContext> server)
		{
			_dispatcherLock = dispatcherLock ?? throw new ArgumentNullException(nameof(dispatcherLock));
			_queue = queue ?? throw new ArgumentNullException(nameof(queue)); ;
			_server = server ?? throw new ArgumentNullException(nameof(server)); ;
		}

		/// <summary>
		/// Execute the dispatcher and process the taskqueue
		/// </summary>
		/// <param name="context"></param>
		public void Execute(IProcessorContext context)
		{
			// check if a thread is allready processing the queue
			if (_dispatcherLock.IsLocked())
			{
				return;
			}

			_dispatcherLock.Lock();

			while (_queue.TryDequeue(out var task))
			{
				task.SetState(TaskState.Dequeued);

				try
				{
					_server.StartNew(new TaskExecutionDispatcher(task));
				}
				catch (Exception e)
				{
					//_logger.Write($"Error processing eveng{Environment.NewLine}EventId: {@event.Id}{Environment.NewLine}Pipeline: {PipelineId}{Environment.NewLine}{e.Message}", Category.Log, LogLevel.Error, "EventBus");
				}
			}

			_dispatcherLock.Unlock();
		}


	}
}
