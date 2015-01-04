using Broadcast.EventSourcing;
using System;
using System.Linq.Expressions;

namespace Broadcast
{
    public interface IBroadcaster
    {
        void Send(Expression<Action> task);

        void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback);

        void Send<T>(Expression<Func<T>> notification) where T : INotification;

        IProcessorContext Context { get; }
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
            Context.Tasks = store;
        }

        public Broadcaster(ProcessorMode mode, ITaskStore store)
        {
            Context.Mode = mode;
            Context.Tasks = store;
        }

        //public Broadcaster(IProcessorContext context)
        //{
        //    _context = context;
        //}

        IProcessorContext _context;
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

        public void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback)
        {
            throw new NotImplementedException();
        }








        public void Send<T>(Expression<Func<T>> notification) where T : INotification
        {
            var backgroundTask = TaskFactory.CreateTask(notification);

            using (var processor = Context.Open())
            {
                processor.Process(backgroundTask);
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
