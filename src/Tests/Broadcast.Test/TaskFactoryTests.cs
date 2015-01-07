﻿using Broadcast.EventSourcing;
using Moq;
using NUnit.Framework;
namespace Broadcast.Test
{
    [TestFixture]
    public class TaskFactoryTests
    {
        [Test]
        public void TaskFactoryNotificationWithNewObjectExpressionTest()
        {
            var task = TaskFactory.CreateTask(() => new Notification());
            Assert.IsNotNull(task);
        }

        [Test]
        public void TaskFactoryNotificationWithObjectReferenceExpressionTest()
        {
            var message = new Notification();
            var task = TaskFactory.CreateTask(() => message);
            Assert.IsNotNull(task);
        }

        [Test]
        public void TaskFactoryNotificationWithMethodReferenceExpressionTest()
        {
            var task = TaskFactory.CreateTask(() => CreateNotification());
            Assert.IsNotNull(task);
        }

        [Test]
        public void TaskFactoryWithMethodReferenceExpressionTest()
        {
            var messageMock = new Mock<IMessage>();
            var task = TaskFactory.CreateTask(() => messageMock.Object.DoSomething("", "", ""));

            Assert.IsNotNull(task);
        }

        [Test]
        public void TaskFactoryNotificationWithMethodInObjectExpressionTest()
        {
            var messageMock = new Mock<IMessage>();
            var task = TaskFactory.CreateTask(() => messageMock.Object.ReturnNotification("", "", ""));

            Assert.IsNotNull(task);
        }


        Notification CreateNotification()
        {
            return new Notification();
        }

        class Notification : INotification
        {
        }

        interface IMessage
        {
            void DoSomething(string value1, string value2, string value3);

            Notification ReturnNotification(string value1, string value2, string value3);
        }
    }
}
