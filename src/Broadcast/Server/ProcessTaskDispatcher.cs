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

		/// <summary>
		/// Creates a new instance of the ProcessTaskDispatcher
		/// </summary>
		/// <param name="broadcaster"></param>
		public ProcessTaskDispatcher(IBroadcaster broadcaster)
		{
			_broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
			_logger = LoggerFactory.Create();
		}

		/// <summary>
		/// Execute the Dispatcher to processes the task.
		/// The Task is only processesed if it is not a scheduled task and is not a recurring task
		/// </summary>
		/// <param name="task"></param>
		public void Execute(ITask task)
		{
			if(task.Time == null && !task.IsRecurring)
			{
				if (task.State == TaskState.Deleted)
				{
					_logger.Write($"Task {task.Id} is marked as deleted and will not be processed by the ProcessTaskDipatcher", LogLevel.Warning);
					return;
				}

				_broadcaster.Process(task);
			}
		}

		/// <summary>
		/// Dispose the Dispatcher
		/// </summary>
		public void Dispose()
		{
		}
	}
}
