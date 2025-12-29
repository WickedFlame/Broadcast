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
            var act = () => new Dispatcher(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Dispatcher_Register()
        {
            var dispatcher = new Dispatcher();
            var handler = new CountEventHandler();

            // Act
            dispatcher.Register<CountEvent>(handler);

            dispatcher.Handlers.Single().Should().Be(handler);
        }

        [Test]
        public void Dispatcher_Register_Multiple()
        {
            var dispatcher = new Dispatcher();

            // Act
            dispatcher.Register<CountEvent>(new CountEventHandler());
            dispatcher.Register<CountEvent>(new CountMessageHandlerTwo());

            dispatcher.Handlers.Should().HaveCount(2);
        }

        [Test]
        public void Dispatcher_Publish()
        {
            var dispatcher = new Dispatcher();
            var handler = new CountEventHandler();
            dispatcher.Register<CountEvent>(handler);

            // Act
            dispatcher.Publish(new CountEvent());

            handler.Count.Should().Be(1);
        }

        [Test]
        public void Dispatcher_EventBus_Publish()
        {
            var eventBus = new Mock<IEventBus>();
            var dispatcher = new Dispatcher(eventBus.Object);

            // Act
            dispatcher.Publish(new CountEvent());

            eventBus.Verify(eb => eb.Publish(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void Dispatcher_Publish_Multiple()
        {
            var dispatcher = new Dispatcher();
            var handler = new CountEventHandler();
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
            var dispatcher = new Dispatcher();
            var one = new CountEventHandler();
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
            var dispatcher = new Dispatcher();
            var handler = new CountEventHandler();
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
            var dispatcher = new Dispatcher();
            var one = new CountEventHandler();
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
