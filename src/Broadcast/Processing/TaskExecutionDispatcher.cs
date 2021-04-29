﻿using System;
using System.Diagnostics;
using Broadcast.Diagnostics;
using Broadcast.EventSourcing;
using Broadcast.Server;

namespace Broadcast.Processing
{
	/// <summary>
	/// BackgroundDispatcher that processes a task.
	/// If the task is a <see cref="INotification"/> and has registered handlers, the output is passed to all handlers
	/// </summary>
	public class TaskExecutionDispatcher : IBackgroundDispatcher<IProcessorContext>
	{
		private readonly ITask _task;
		private readonly ILogger _logger;

		/// <summary>
		/// Creates a new instance of a TaskExecutionDispatcher
		/// </summary>
		/// <param name="task"></param>
		public TaskExecutionDispatcher(ITask task)
		{
			_task = task ?? throw new ArgumentNullException(nameof(task));

			_logger = LoggerFactory.Create();
			_logger.Write($"Starting new TaskExecutionDispatcher for {task.Id}");
		}

		/// <summary>
		/// Execute the dispatcher and process the task
		/// </summary>
		/// <param name="context"></param>
		public void Execute(IProcessorContext context)
		{
			// Stopwatch is the only object allowed outside the try..catch to ensure no errors occure outside the executionblock
			var sw = new Stopwatch();
			sw.Start();

			try
			{
				_logger.Write($"Start processing task {_task.Id}");
				_task.SetInprocess();

				//TODO: INotification is bad design. any object should be useable
				var invocation = new TaskInvocation();
				var output = _task.Invoke(invocation);

				_task.SetProcessed();
			}
			catch (Exception e)
			{
				_task.SetState(TaskState.Faulted);
				_logger.Write($"Task execution failed for {_task.Id}", e);
			}
			finally
			{
				sw.Stop();
				//TODO: Write metrics here

				_logger.Write($"End processing task {_task.Id}. Duration {sw.ElapsedMilliseconds} ms");
			}
		}
	}
}
