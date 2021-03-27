using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Broadcast.Test
{
    [TestFixture]
    public class NotificationTests
    {
        [Test]
        public void DefaultNotificationTests()
        {
            var broadcaster = new Broadcaster();
            var notificationHandler = new NotificationHandler();
            var delegateHandler = new DelegateHandler();
            int expressionHandler = 0;


            broadcaster.RegisterHandler(notificationHandler);
            broadcaster.RegisterHandler<Message>(delegateHandler.Handle);
            broadcaster.RegisterHandler<Message>(a => expressionHandler = a.ID);

            broadcaster.Send(() => new Message(5));

            broadcaster.WaitAll();

            Assert.IsTrue(notificationHandler.ID == 5);
            Assert.IsTrue(delegateHandler.ID == 5);
            Assert.IsTrue(expressionHandler == 5);
        }

        //[Test]
        //public async Task NotificationAsyncTests()
        //{
        //    var broadcaster = new Broadcaster();
        //    var notificationHandler = new NotificationHandler();
        //    var delegateHandler = new DelegateHandler();
        //    int expressionHandler = 0;


        //    broadcaster.RegisterHandler(notificationHandler);
        //    broadcaster.RegisterHandler<Message>(delegateHandler.Handle);
        //    broadcaster.RegisterHandler<Message>(a => expressionHandler = a.ID);

        //    await broadcaster.SendAsync(() => new Message(5));

        //    Assert.IsTrue(notificationHandler.ID == 5);
        //    Assert.IsTrue(delegateHandler.ID == 5);
        //    Assert.IsTrue(expressionHandler == 5);
        //}

        //[Test]
        //public async Task NotificationAsyncWithMultipleHandlersTests()
        //{
        //    var broadcaster = new Broadcaster(ProcessorMode.Serial);
        //    int handlerOne = 0;
        //    int handlerTwo = 0;

        //    broadcaster.RegisterHandler<Message>(a => handlerOne = a.ID);
        //    broadcaster.RegisterHandler<MessageTwo>(a => handlerTwo = a.ID);

        //    await broadcaster.SendAsync(() => new Message(5));

        //    Assert.IsTrue(handlerOne == 5);
        //    Assert.IsTrue(handlerTwo == 0);
        //}
                
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
