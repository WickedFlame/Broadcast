using Broadcast.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast
{
    public interface IBroadcaster
    {
        void Send(Expression<Action> task);

        void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback);

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
            Context.TaskStore = store;
        }

        public Broadcaster(ProcessorMode mode, ITaskStore store)
        {
            Context.Mode = mode;
            Context.TaskStore = store;
        }

        public Broadcaster(IProcessorContext context)
        {
            _context = context;
        }

        IProcessorContext _context;
        public IProcessorContext Context
        {
            get
            {
                if (_context == null)
                    _context = ProcessorContextFactory.ContextFactory();
                return _context;
            }
        }
      
		
        public void Send(Expression<Action> task)
        {
            var backgroundJob = new BackgroundTask
            {
                Task = task,
                State = TaskState.New
            };

            using (var processor = Context.Open())
            {
                processor.Process(backgroundJob);
            }
        }

        public void Send<T>(Expression<Func<T>> task, Expression<Action<T>> callback)
        {
            throw new NotImplementedException();
        }
    }
}
