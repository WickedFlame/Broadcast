using System;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    public class DispatcherTests
    {
        [Test]
        public void Dispatcher_Null_EventBus()
        {
            var act = () => new Dispatcher<IEvent>(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Dispatcher_Register()
        {
            var dispatcher = new Dispatcher<IEvent>();
            var handler = new CountMessageHandler();

            // Act
            dispatcher.Register<CountEvent>(handler);

            dispatcher.Handlers.Single().Should().Be(handler);
        }

        [Test]
        public void Dispatcher_Register_Multiple()
        {
            var dispatcher = new Dispatcher<IEvent>();

            // Act
            dispatcher.Register<CountEvent>(new CountMessageHandler());
            dispatcher.Register<CountEvent>(new CountMessageHandlerTwo());

            dispatcher.Handlers.Should().HaveCount(2);
        }

        [Test]
        public void Dispatcher_Publish()
        {
            var dispatcher = new Dispatcher<IEvent>();
            var handler = new CountMessageHandler();
            dispatcher.Register<CountEvent>(handler);

            // Act
            dispatcher.Publish(new CountEvent());

            handler.Count.Should().Be(1);
        }

        [Test]
        public void Dispatcher_EventBus_Publish()
        {
            var eventBus = new Mock<IEventBus>();
            var dispatcher = new Dispatcher<IEvent>(eventBus.Object);

            // Act
            dispatcher.Publish(new CountEvent());

            eventBus.Verify(eb => eb.Publish(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<IEvent>()), Times.Once);
        }

        [Test]
        public void Dispatcher_Publish_Multiple()
        {
            var dispatcher = new Dispatcher<IEvent>();
            var handler = new CountMessageHandler();
            dispatcher.Register<CountEvent>(handler);

            // Act
            for (var i = 1; i < 100; i++)
            {
                dispatcher.Publish(new CountEvent());

                handler.Count.Should().Be(i);
            }
        }

        [Test]
        public void Dispatcher_Publish_Multiple_Handlers()
        {
            var dispatcher = new Dispatcher<IEvent>();
            var one = new CountMessageHandler();
            dispatcher.Register<CountEvent>(one);

            var two = new CountMessageHandlerTwo();
            dispatcher.Register<CountEvent>(two);

            // Act
            dispatcher.Publish(new CountEvent());

            one.Count.Should().Be(1);
            two.Count.Should().Be(1);
        }

        [Test]
        public void Dispatcher_Enqueue()
        {
            var dispatcher = new Dispatcher<IEvent>();
            var handler = new CountMessageHandler();
            dispatcher.Register<CountEvent>(handler);

            // Act
            dispatcher.Enqueue(new CountEvent());

            while (dispatcher.Queue.Any())
            {
                Task.Delay(50).Wait();
            }

            handler.Count.Should().Be(1);
        }

        [Test]
        public void Dispatcher_Enqueue_Multiple_Handlers()
        {
            var dispatcher = new Dispatcher<IEvent>();
            var one = new CountMessageHandler();
            dispatcher.Register<CountEvent>(one);

            var two = new CountMessageHandlerTwo();
            dispatcher.Register<CountEvent>(two);

            // Act
            dispatcher.Enqueue(new CountEvent());

            while (dispatcher.Queue.Any())
            {
                Task.Delay(50).Wait();
            }

            one.Count.Should().Be(1);
            two.Count.Should().Be(1);
        }
    }
}
