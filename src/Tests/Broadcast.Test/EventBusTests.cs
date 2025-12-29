using System;
using System.Linq;

namespace Broadcast.Test
{
    public class EventBusTests
    {
        [Test]
        public void EventBus_Null_EventStore()
        {
            var act = () => new EventBus(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EventBus_Subscribe()
        {
            var eventBus = new EventBus();

            var handler = new CountEventHandler();
            eventBus.Subscribe<CountEvent>(handler);

            eventBus.Handlers.Single().Should().Be(handler);
        }

        [Test]
        public void EventBus_Send()
        {
            var eventBus = new EventBus();

            var firstHandler = new CountEventHandler();
            eventBus.Subscribe<CountEvent>(firstHandler);

            var secondHandler = new CountMessageHandlerTwo();
            eventBus.Subscribe<CountEvent>(secondHandler);

            // Act
            eventBus.Send(new CountEvent());

            firstHandler.Count.Should().Be(1);
            secondHandler.Count.Should().Be(1);
        }

        [Test]
        public void EventBus_Send_BaseType()
        {
            //
            // This should trigger the TryHandle because the event is sent as the base interface type instead of the registered type

            var eventBus = new EventBus();

            var firstHandler = new CountEventHandler();
            eventBus.Subscribe<CountEvent>(firstHandler);

            var secondHandler = new CountMessageHandlerTwo();
            eventBus.Subscribe<CountEvent>(secondHandler);

            // Act
            eventBus.Send(new CountEvent() as IEvent);

            firstHandler.Count.Should().Be(1);
            secondHandler.Count.Should().Be(1);
        }

        [Test]
        public void EventBus_Send_No_Handler()
        {
            var eventBus = new EventBus();

            // Act
            var act = () => eventBus.Send(new CountEvent());

            act.Should().NotThrow();
        }

        [Test]
        public void EventBus_Publish()
        {
            var eventBus = new EventBus();

            var firstHandler = new CountEventHandler();
            eventBus.Subscribe<CountEvent>(firstHandler);

            var secondHandler = new CountMessageHandlerTwo();
            eventBus.Subscribe<CountEvent>(secondHandler);

            // Act
            eventBus.Publish("one", DateTime.UtcNow, new CountEvent());

            firstHandler.Count.Should().Be(1);
            secondHandler.Count.Should().Be(1);
        }

        [Test]
        public void EventBus_Publish_EventStore()
        {
            var eventStore = new Mock<IEventStore>();
            var eventBus = new EventBus(eventStore.Object);

            eventBus.Subscribe<CountEvent>(new CountEventHandler());

            // Act
            eventBus.Publish("one", DateTime.UtcNow, new CountEvent());

            eventStore.Verify(es => es.Add("one", It.IsAny<DateTime>(), It.IsAny<IEvent>()), Times.Once);
        }
    }
}
