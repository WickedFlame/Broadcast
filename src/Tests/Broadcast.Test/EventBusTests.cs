using Broadcast.Test;

namespace Broadcast.Test
{
    public class EventBusTests
    {
        [Test]
        public void EventBus_Send()
        {
            var eventBus = new EventBus();

            var firstHandler = new CountMessageHandler();
            eventBus.Subscribe<CountEvent>(firstHandler);

            var secondHandler = new CountMessageHandlerTwo();
            eventBus.Subscribe<CountEvent>(secondHandler);

            // Act
            eventBus.Send(new CountEvent());

            firstHandler.Count.Should().Be(1);
            secondHandler.Count.Should().Be(1);
        }
    }
}
