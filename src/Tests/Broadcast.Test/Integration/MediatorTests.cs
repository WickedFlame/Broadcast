using System;
using NUnit;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Broadcast.Test
{
    [TestFixture]
    public class MediatorTests
    {
        [Test]
        public void MediatorNotificationTests()
        {
            var mediator = new Mediator();
            var notificationHandler = new NotificationHandler();
            var delegateHandler = new DelegateHandler();
            int expressionHandler = 0;


            mediator.RegisterHandler(notificationHandler);
            mediator.RegisterHandler<Message>(delegateHandler.Handle);
            mediator.RegisterHandler<Message>(a => expressionHandler = a.ID);

            mediator.Send(() => new Message(5));

            Assert.IsTrue(notificationHandler.ID == 5);
            Assert.IsTrue(delegateHandler.ID == 5);
            Assert.IsTrue(expressionHandler == 5);
        }

        [Test]
        public async Task MediatorAsyncNotificationTests()
        {
            var mediator = new Mediator();
            var notificationHandler = new NotificationHandler();
            var delegateHandler = new DelegateHandler();
            int expressionHandler = 0;

            mediator.RegisterHandler(notificationHandler);
            mediator.RegisterHandler<Message>(delegateHandler.Handle);
            mediator.RegisterHandler<Message>(a => expressionHandler = a.ID);

            await mediator.SendAsync(() => new Message(5));

            Assert.IsTrue(notificationHandler.ID == 5);
            Assert.IsTrue(delegateHandler.ID == 5);
            Assert.IsTrue(expressionHandler == 5);
        }

        [Test]
        public async Task MediatorAsyncWithMultipleHandlersTests()
        {
            var mediator = new Mediator();
            int handlerOne = 0;
            int handlerTwo = 0;

            mediator.RegisterHandler<Message>(a => handlerOne = a.ID);
            mediator.RegisterHandler<MessageTwo>(a => handlerTwo = a.ID);

            await mediator.SendAsync(() => new Message(5));

            Assert.IsTrue(handlerOne == 5);
            Assert.IsTrue(handlerTwo == 0);
        }

        [Test]
        public async Task MediatorInFalseModeTest()
        {
            var mediator = new Mediator();
            mediator.Context.Mode = ProcessorMode.Async;

            int expressionHandler = 0;

            mediator.RegisterHandler<Message>(a => expressionHandler = a.ID);

            try
            {
                await mediator.SendAsync(() => new Message(5));
            }
            catch (InvalidOperationException)
            {
                return;
            }

            Assert.Fail();
        }

        class Message : INotification
        {
            public Message(int id)
            {
                ID = id;
            }

            public int ID { get; private set; }
        }

        class MessageTwo : INotification
        {
            public MessageTwo(int id)
            {
                ID = id;
            }

            public int ID { get; private set; }
        }

        class NotificationHandler : INotificationTarget<Message>
        {
            public void Handle(Message notification)
            {
                ID = notification.ID;
            }

            public int ID { get; set; }
        }

        class DelegateHandler
        {
            public void Handle(Message notification)
            {
                ID = notification.ID;
            }

            public int ID { get; set; }
        }
    }
}
