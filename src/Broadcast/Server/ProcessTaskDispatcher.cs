using System;
using Broadcast.Diagnostics;
using Broadcast.EventSourcing;

namespace Broadcast.Server
{
	/// <summary>
	/// Dispatcher that dispatches tasks to the <see cref="IBroadcaster"/> to be processed
	/// </summary>
	public class ProcessTaskDispatcher : IDispatcher
	{
		private readonly IBroadcaster _broadcaster;
		private readonly ILogger _logger;
		private readonly ITaskStore _store;

		/// <summary>
		/// Creates a new instance of the ProcessTaskDispatcher
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="store"></param>
		public ProcessTaskDispatcher(IBroadcaster broadcaster, ITaskStore store)
		{
			_broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
			_store = store ?? throw new ArgumentNullException(nameof(store));
			_logger = LoggerFactory.Create();
		}

		/// <summary>
		/// Execute the Dispatcher to processes the task.
		/// The Task is only processesed if it is not a scheduled task and is not a recurring task
		/// </summary>
		/// <param name="task"></param>
		public void Execute(ITask task)
		{
			if(task.TaskType == TaskType.Simple && task.Time == null && !task.IsRecurring)
			{
				if (task.State == TaskState.Deleted)
				{
					_logger.Write($"Task {task.Id} is marked as deleted and will not be processed by the ProcessTaskDipatcher", LogLevel.Warning);
					return;
				}

				// Set the queue/server to where the task is working on
				_store.AssignTaskToQueue(task.Id, _broadcaster.Name);

				_broadcaster.Process(task);

				// remove the task from the queue
				_store.RemoveTaskFromQueue(task.Id, _broadcaster.Name);
			}
		}

		/// <summary>
		/// Dispose the Dispatcher
		/// </summary>
		public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
			// clear all references here
        }
	}
}
