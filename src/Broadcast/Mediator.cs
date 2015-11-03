using System.Text;
using Broadcast.EventSourcing;
using System;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Broadcast
{
    public class Mediator : IMediator
    {
        IProcessorContext _context;
        public IProcessorContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = ProcessorContextFactory.GetContext();
                    _context.Mode = ProcessorMode.Default;
                }

                return _context;
            }
        }

        public Mediator()
        {
        }

        /// <summary>
        /// Register a <see cref="INotificationTarget"/> that gets called when the specific <see cref="INotification"/> is published
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification
        {
            RegisterHandler<T>(a => target.Handle(a));
        }

        /// <summary>
        /// Register a delegate that gets called when the specific <see cref="INotification"/> is published
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        public void Send<T>(Expression<Func<T>> notification) where T : INotification
        {
            var task = TaskFactory.CreateTask(notification);
            using (var processor = Context.Open())
            {
                processor.Process(task);
            }
        }
        
        /// <summary>
        /// Publishes a <see cref="INotification"/> and passes it to the registered NotificationTargets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
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
                await System.Threading.Tasks.Task.Run(() => processor.Process(task));
            }
        }
    }
}
