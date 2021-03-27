using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    /// <summary>
    /// A class that can process different kinds of delegates and notifications
    /// </summary>
    public class TaskProcessor : ITaskProcessor, IDisposable
    {
	    private static readonly object ProcessorLock = new object();
	    private bool _inProcess = false;

		private readonly ITaskStore _store;
        private readonly INotificationHandlerStore _handlers;
        private readonly TaskList _taskList = new TaskList();

        public TaskProcessor(ITaskStore store, INotificationHandlerStore handlers)
        {
            _store = store;
            _handlers = handlers;
        }

        protected ITaskStore Store => _store;

        protected INotificationHandlerStore Handlers => _handlers;

        /// <summary>
        /// Add a delegate handler to the store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void AddHandler<T>(Action<T> target) where T : INotification
        {
            Handlers.AddHandler(target);
        }

        public void WaitAll()
        {
	        _taskList.WaitAll();
        }

		/// <summary>
		/// Process the delegate task
		/// </summary>
		/// <param name="task">The task to process</param>
		public void Process(ITask task)
        {
	        Store.Add(task);

	        // check if a thread is allready processing the queue
	        if (_inProcess)
	        {
		        return;
	        }

			// start new background thread to process all queued tasks
			var thread = Task.Run(() => ProcessTasks());
			_taskList.Add(thread);
		}





        private void ProcessTasks()
        {
	        // check if a thread is allready processing the queue
	        if (_inProcess)
	        {
		        return;
	        }

	        lock (ProcessorLock)
	        {
		        _inProcess = true;

		        while (Store.TryDequeue(out var task))
		        {
			        try
			        {
				        //TODO: Create own TaskScheduler and store in options
				        var taskScheduler = TaskScheduler.Default;
				        var thread = Task.Factory.StartNew(() => ProcessItem(task), CancellationToken.None, TaskCreationOptions.None, taskScheduler);
				        _taskList.Add(thread);
					}
			        catch (Exception e)
			        {
				        //_logger.Write($"Error processing eveng{Environment.NewLine}EventId: {@event.Id}{Environment.NewLine}Pipeline: {PipelineId}{Environment.NewLine}{e.Message}", Category.Log, LogLevel.Error, "EventBus");
			        }
		        }

				_inProcess = false;
	        }
        }

		protected virtual string ProcessItem(ITask task)
        {
            Store.SetInprocess(task);

            try
            {
				//TODO: INotification is bad design. any object should be useable
				var invocation = new TaskInvocation();
				var output = task.Invoke(invocation) as INotification;

				// try to find the handlers
				if (output != null && Handlers.Handlers.TryGetValue(output.GetType(), out var handlers))
				{
					//// it could be that T is of a base/inherited type but the handler is of a object type
					//if (!Handlers.Handlers.TryGetValue(output.GetType(), out handlers))
					//{
					//	return;
					//}

					// run all handlers with the value
					foreach (var handler in handlers)
					{
						handler(output);
					}
				}
			}
            catch (Exception ex)
            {
                //TODO: set taskt to faulted
                //TODO: log exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            Store.SetProcessed(task);
            return task.Id;
        }
		
        public void Dispose()
        {
        }
    }
}
