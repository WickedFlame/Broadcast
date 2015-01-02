using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    [TestClass]
    public class MediatorTests
    {
        [TestMethod]
        public void MediatorNotificationTests()
        {
            var mediator = new Mediator();
            var notificationHandler = new NotificationHandler();
            var delegateHandler = new DelegateHandler();
            int expressionHandler = 0;


            mediator.RegisterHandler(notificationHandler);
            mediator.RegisterHandler<Message>(delegateHandler.Handle);
            mediator.RegisterHandler<Message>(a => expressionHandler = a.ID);

            mediator.Publish(new Message(5));

            Assert.IsTrue(notificationHandler.ID == 5);
            Assert.IsTrue(delegateHandler.ID == 5);
            Assert.IsTrue(expressionHandler == 5);
        }

        [TestMethod]
        public async Task MediatorAsyncNotificationTests()
        {
            var mediator = new Mediator();
            var notificationHandler = new NotificationHandler();
            var delegateHandler = new DelegateHandler();
            int expressionHandler = 0;

            mediator.RegisterHandler(notificationHandler);
            mediator.RegisterHandler<Message>(delegateHandler.Handle);
            mediator.RegisterHandler<Message>(a => expressionHandler = a.ID);

            await mediator.PublishAsync(new Message(5));

            Assert.IsTrue(notificationHandler.ID == 5);
            Assert.IsTrue(delegateHandler.ID == 5);
            Assert.IsTrue(expressionHandler == 5);
        }



class Message : INotification
{
    public Message(int id)
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
