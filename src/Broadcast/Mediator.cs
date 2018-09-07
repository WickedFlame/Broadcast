using System;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Task = System.Threading.Tasks.Task;

namespace Broadcast
{
    public class Mediator : IMediator
    {
        private IProcessorContext _context;

        public IProcessorContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = ProcessorContextFactory.GetContext();
                    _context.Mode = ProcessorMode.Parallel;
                }
                return _context;
            }
        }
        
        /// <summary>
        /// Register a <see cref="INotificationTarget"/> that gets called when the specific <see cref="INotification"/> is published
        /// </summary>
        /// <typeparam name="T">Type of notification</typeparam>
        /// <param name="target">The target</param>
        public void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification
        {
            RegisterHandler<T>(a => target.Handle(a));
        }

        /// <summary>
        /// Register a delegate that gets called when the specific <see cref="INotification"/> is published
        /// </summary>
        /// <typeparam name="T">Type of notification</typeparam>
        /// <param name="target">The target</param>
        public void RegisterHandler<T>(Action<T> target) where T : INotification
        {
            using (var processor = Context.Open())
            {
                processor.AddHandler(target);
            }
        }

        /// <summary>
        /// Publishes a <see cref="INotification"/> and passes it to the registered NotificationTargets
        /// </summary>
        /// <typeparam name="T">Type of notification</typeparam>
        /// <param name="notification">The Notification</param>
        public void Send<T>(Func<T> notification) where T : INotification
        {
            var task = TaskFactory.CreateNotifiableTask(notification);
            using (var processor = Context.Open())
            {
                processor.Process(task);
            }
        }

        /// <summary>
        /// Publishes a <see cref="INotification"/> and passes it to the registered NotificationTargets
        /// </summary>
        /// <typeparam name="T">Type of notification</typeparam>
        /// <param name="notification">The Notification</param>
        /// <returns>Async Task</returns>
        public async Task SendAsync<T>(Func<T> notification) where T : INotification
        {
            // async 
            var task = TaskFactory.CreateNotifiableTask(notification);
            using (var processor = new AsyncTaskProcessor(Context.Store, Context.NotificationHandlers))
            {
                await processor.ProcessAsync(task);
            }
        }
    }
}
