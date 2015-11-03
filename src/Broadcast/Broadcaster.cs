using Broadcast.EventSourcing;
using System;
using System.Linq.Expressions;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace Broadcast
{
    public class Broadcaster : IBroadcaster
    {
        public Broadcaster()
        {
        }

        public Broadcaster(ProcessorMode mode)
        {
            Context.Mode = mode;
        }

        public Broadcaster(ITaskStore store)
        {
            Context.Store = store;
        }

        public Broadcaster(ProcessorMode mode, ITaskStore store)
        {
            Context.Mode = mode;
            Context.Store = store;
        }

        private IProcessorContext _context;

        /// <summary>
        /// Gets the ProcessorContext that containes all information create a TaskProcessor
        /// </summary>
        public IProcessorContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = ProcessorContextFactory.GetContext();
                }

                return _context;
            }
        }

        /// <summary>
        /// Send a delegate to the task processor
        /// </summary>
        /// <param name="action"></param>
        public void Send(Expression<Action> action)
        {
            var task = TaskFactory.CreateTask(action);
            using (var processor = Context.Open())
            {
                processor.Process(task);
            }
        }

        //public void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Sends a INotification to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        public void Send<T>(Expression<Func<T>> notification) where T : INotification
        {
            var task = TaskFactory.CreateTask(notification);
            using (var processor = Context.Open())
            {
                processor.Process(task);
            }
        }

        /// <summary>
        /// Sends a INotification async to the processor. The INotification will be passed to all registered Handlers of the same type. Async method calls are only allowed in ProcessorMode.Default
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <returns></returns>
        public async Task SendAsync<T>(Expression<Func<T>> notification) where T : INotification
        {
            if (Context.Mode != ProcessorMode.Default)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Async message handling is only alowed when running the broadcast context in Default Mode.");
                sb.AppendLine("Use Send(message) if ProcessorMode is intentialy run in another mode than ProcessorMode.Default");
                throw new InvalidOperationException(sb.ToString());
            }

            var task = TaskFactory.CreateTask(notification);
            using (var processor = Context.Open())
            {
                await Task.Run(() => processor.Process(task));
            }
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
            using (var processor = Context.Open())
            {
                processor.AddHandler(target);
            }
        }
    }
}
