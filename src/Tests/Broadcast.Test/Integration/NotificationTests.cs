﻿using System;
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
            var mediator = new Broadcaster(ProcessorMode.Background);
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

        //[Test]
        //public async Task NotificationAsyncTests()
        //{
        //    var mediator = new Broadcaster();
        //    var notificationHandler = new NotificationHandler();
        //    var delegateHandler = new DelegateHandler();
        //    int expressionHandler = 0;


        //    mediator.RegisterHandler(notificationHandler);
        //    mediator.RegisterHandler<Message>(delegateHandler.Handle);
        //    mediator.RegisterHandler<Message>(a => expressionHandler = a.ID);

        //    await mediator.SendAsync(() => new Message(5));

        //    Assert.IsTrue(notificationHandler.ID == 5);
        //    Assert.IsTrue(delegateHandler.ID == 5);
        //    Assert.IsTrue(expressionHandler == 5);
        //}

        //[Test]
        //public async Task NotificationAsyncWithMultipleHandlersTests()
        //{
        //    var mediator = new Broadcaster(ProcessorMode.Serial);
        //    int handlerOne = 0;
        //    int handlerTwo = 0;

        //    mediator.RegisterHandler<Message>(a => handlerOne = a.ID);
        //    mediator.RegisterHandler<MessageTwo>(a => handlerTwo = a.ID);

        //    await mediator.SendAsync(() => new Message(5));

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
