using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    /// <summary>
    /// A class that can process different kinds of delegates and notifications
    /// </summary>
    public abstract class TaskProcessorBase : ITaskProcessor, IDisposable
    {
        private readonly ITaskStore _store;
        private readonly INotificationHandlerStore _handlers;

        public TaskProcessorBase(ITaskStore store, INotificationHandlerStore handlers)
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

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <param name="task"></param>
        public abstract void Process(ITask task);

        protected virtual void ProcessItem(ITask task)
        {
            Store.SetInprocess(task);

            try
            {
                //task.Task.Compile().Invoke();

                //var invocation = new TaskInvocation();
                //invocation.InvokeTask(task);

                if (task is ExpressionTask expression)
                {
	                var invocation = new TaskInvocation();
	                invocation.InvokeTask(expression);
				}
                else if (task is ActionTask action)
                {
	                action.Task.Invoke();
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
				T item = default;

				// get the job item from the task
				if(task is ExpressionTask<T> expression)
				{
					item = expression.Task.Compile().Invoke();
				}
				else if (task is DelegateTask<T> deleg)
				{
					item = deleg.Task.Invoke();
				}


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
