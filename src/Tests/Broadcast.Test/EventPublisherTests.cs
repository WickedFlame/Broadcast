using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Test
{
    public class EventPublisherTests
    {
        private HandlerSubscriptionCollection _subscriptions;
        private EventPublisher _publisher;

        [SetUp]
        public void Setup()
        {
            _subscriptions = new HandlerSubscriptionCollection();
            _publisher = new EventPublisher(_subscriptions);
        }

        [Test]
        public void EventPublisher_Publish()
        {
            var handler = new CountMessageHandler();
            _subscriptions.Add(typeof(CountEvent), handler);
            
            _publisher.Publish(new CountEvent());

            handler.Count.Should().Be(1);
        }

        [Test]
        public void EventPublisher_Publish_No_Handler()
        {
            // Act
            var act = () => _publisher.Publish(new CountEvent());
            act.Should().NotThrow();
        }

        [Test]
        public void EventPublisher_Publish_No_Handler_Return()
        {
            // Act
            _publisher.Publish(new CountEvent())
                .Should().BeFalse();
        }

        [Test]
        public void EventPublisher_Publish_BaseType()
        {
            //
            // This should trigger the TryHandle because the event is sent as the base interface type instead of the registered type

            var handler = new CountMessageHandler();
            _subscriptions.Add(typeof(CountEvent), handler);

            _publisher.Publish(new CountEvent() as IEvent);

            handler.Count.Should().Be(1);
        }

        [Test]
        public void EventPublisher_Publish_Multiple_Handlers()
        {
            var one = new CountMessageHandler();
            _subscriptions.Add(typeof(CountEvent), one);

            var two = new CountMessageHandler();
            _subscriptions.Add(typeof(CountEvent), two);

            _publisher.Publish(new CountEvent());

            one.Count.Should().Be(1);
            two.Count.Should().Be(1);
        }
    }
}
