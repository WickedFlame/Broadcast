using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Integration.Test
{
    public class DispatcherWorkflowTests
    {
        [Test]
        public void Workflow_Dispatcher_Direct()
        {
            var dispatcher = new Dispatcher<IWorkflowEvent>();

            var handler = new WorkflowModelHandler();
            dispatcher.Register<FirstWorkflowEvent>(handler);
            dispatcher.Register<SecondWorkflowEvent>(handler);

            dispatcher.Publish(new FirstWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(0);

            dispatcher.Publish(new SecondWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(1);
        }

        [Test]
        public void Workflow_Dispatcher_EventBus()
        {
            var eventBus = new EventBus();
            var handler = new WorkflowModelHandler();
            eventBus.Subscribe<FirstWorkflowEvent>(handler);
            eventBus.Subscribe<SecondWorkflowEvent>(handler);

            var dispatcher = new Dispatcher<IWorkflowEvent>(eventBus);

            

            dispatcher.Publish(new FirstWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(0);

            dispatcher.Publish(new SecondWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(1);
        }

        [Test]
        public void Workflow_Dispatcher_EventBus_MultipleHandlers()
        {
            var eventBus = new EventBus();
            var one = new CountEventHandler();
            eventBus.Subscribe<CountEvent>(one);

            var two = new CountMessageHandlerTwo();
            eventBus.Subscribe<CountEvent>(two);

            var dispatcher = new Dispatcher<IEvent>(eventBus);

            dispatcher.Publish(new CountEvent());

            one.Count.Should().Be(1);
            two.Count.Should().Be(1);
        }
    }

    public interface IWorkflowEvent
    {
    }

    public class FirstWorkflowEvent : IWorkflowEvent, IEvent
    {
    }

    public class SecondWorkflowEvent : IWorkflowEvent, IEvent
    {
    }

    public class WorkflowModelHandler : 
        IEventHandler<FirstWorkflowEvent>,
        IEventHandler<SecondWorkflowEvent>
    {
        public int First { get; private set; }
        
        public int Second { get; private set; }

        public void Handle(FirstWorkflowEvent message)
        {
            First++;
        }

        public void Handle(SecondWorkflowEvent @event)
        {
            Second++;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
