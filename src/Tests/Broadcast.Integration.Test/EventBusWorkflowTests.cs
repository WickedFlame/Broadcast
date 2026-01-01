using Moq;
using System;

namespace Broadcast.Integration.Test
{
    public class EventBusWorkflowTests
    {
        [Test]
        public void Workflow_EventBus_Send()
        {
            var eventBus = new EventBus();

            var handler = new WorkflowModelHandler();
            eventBus.Subscribe<FirstWorkflowEvent>(handler);
            eventBus.Subscribe<SecondWorkflowEvent>(handler);

            // Act
            eventBus.Send(new FirstWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(0);

            // Act
            eventBus.Send(new SecondWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(1);
        }

        [Test]
        public void Workflow_EventBus_Publish()
        {
            var eventStore = new Mock<IEventStore>();
            var eventBus = new EventBus(eventStore.Object);

            var handler = new WorkflowModelHandler();
            eventBus.Subscribe<FirstWorkflowEvent>(handler);
            eventBus.Subscribe<SecondWorkflowEvent>(handler);

            // Act
            eventBus.Publish(new FirstWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(0);

            // Act
            eventBus.Publish(new SecondWorkflowEvent());

            handler.First.Should().Be(1);
            handler.Second.Should().Be(1);

            eventStore.Verify(es => es.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<object>()), Times.Exactly(2));
        }






        //[Test]
        //public void Workflow_EventBus_Send_NonHandler()
        //{
        //    var eventStore = new Mock<IEventStore>();
        //    var eventBus = new EventBus(eventStore.Object);

        //    var handler = new NonGenericMesssageHandler();
        //    eventBus.Subscribe<NonGenericEvent>(handler);

        //    // Act
        //    eventBus.Send(new NonGenericEvent());

        //    handler.First.Should().Be(1);
        //    handler.Second.Should().Be(1);
        //}

        //public class NonGenericMesssageHandler : IMessageHandler
        //{
        //    public void Handle(NonGenericEvent message)
        //    {
        //    }

        //    public void Dispose()
        //    {
        //    }
        //}

        //public class NonGenericEvent { }
    }
}
