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
				        var _taskScheduler = TaskScheduler.Default;
				        var thread = Task.Factory.StartNew(() => ProcessItem(task), CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
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
				var invocation = new TaskInvocation();
				task.Invoke(invocation);
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

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        public virtual void Process<T>(ITask<T> task) where T : INotification
        {
            Store.Add(task);

            // mark the task to be in process
            Store.SetInprocess(task);

            try
            {
				//T item = default;

				//// get the job item from the task
				//if(task is ExpressionTask<T> expression)
				//{
				//	item = expression.Task.Compile().Invoke();
				//}
				//else if (task is DelegateTask<T> deleg)
				//{
				//	item = deleg.Task.Invoke();
				//}

				var invocation = new TaskInvocation();
				var item = task.Invoke(invocation) as INotification;


				// try to find the handlers
				if (!Handlers.Handlers.TryGetValue(typeof(T), out var handlers))
                {
                    // it could be that T is of a base/inherited type but the handler is of a object type
                    if (!Handlers.Handlers.TryGetValue(item.GetType(), out handlers))
                    {
                        return;
                    }
                }

                // run all handlers with the value
                foreach (var handler in handlers)
                {
                    handler(item);
                }
            }
            catch (Exception ex)
            {
                //TODO: set taskt to faulted
                //TODO: log exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            // set the task to processed state
            Store.SetProcessed(task);
        }
		
        public void Dispose()
        {
        }
    }
}
