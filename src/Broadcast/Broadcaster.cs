using Broadcast.EventSourcing;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Broadcast
{
    public interface IBroadcaster
    {
        IProcessorContext Context { get; }

        void Send(Expression<Action> task);

        //void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback);

        System.Threading.Tasks.Task SendAsync<T>(Expression<Func<T>> notification) where T : INotification;

        void Send<T>(Expression<Func<T>> notification) where T : INotification;

        void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification;

        void RegisterHandler<T>(Action<T> target) where T : INotification;
    }

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

        public IProcessorContext Context
        {
            get
            {
                if (_context == null)
                    _context = ProcessorContextFactory.GetContext();
                return _context;
            }
        }

        public void Send(Expression<Action> task)
        {
            var backgroundTask = TaskFactory.CreateTask(task);
            using (var processor = Context.Open())
            {
                processor.Process(backgroundTask);
            }
        }

        //public void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback)
        //{
        //    throw new NotImplementedException();
        //}

        public void Send<T>(Expression<Func<T>> notification) where T : INotification
        {
            var backgroundTask = TaskFactory.CreateTask(notification);
            using (var processor = Context.Open())
            {
                processor.Process(backgroundTask);
            }
        }

        public async System.Threading.Tasks.Task SendAsync<T>(Expression<Func<T>> notification) where T : INotification
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

        public void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification
        {
            RegisterHandler<T>(a => target.Handle(a));
        }

        public void RegisterHandler<T>(Action<T> target) where T : INotification
        {
            using (var processor = Context.Open())
            {
                processor.AddHandler(target);
            }
        }
    }
}
