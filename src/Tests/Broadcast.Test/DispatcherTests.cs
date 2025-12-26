using System;

namespace Broadcast.Test
{
    public class DispatcherTests
    {
        [Test]
        public void Dispatcher_EventBus_Send()
        {
            var eventBus = new Mock<IEventBus>();
            var dispatcher = new Dispatcher<IEvent>(eventBus.Object);

            // Act
            dispatcher.Send(new CountEvent());

            eventBus.Verify(eb => eb.Publish(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<IEvent>()), Times.Once);
        }
    }
}
