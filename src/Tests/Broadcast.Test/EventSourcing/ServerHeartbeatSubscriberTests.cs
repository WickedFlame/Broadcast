using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class ServerHeartbeatSubscriberTests
	{
		[Test]
		public void ServerHeartbeatSubscriber_ctor()
		{
			var store = new Mock<ITaskStore>();
			Assert.DoesNotThrow(() => new ServerHeartbeatSubscriber(store.Object));
		}

		[Test]
		public void ServerHeartbeatSubscriber_ctor_Null_Store()
		{
			Assert.Throws<ArgumentNullException>(() => new ServerHeartbeatSubscriber(null));
		}

		[Test]
		public void ServerHeartbeatSubscriber_EventKey()
		{
			var store = new Mock<ITaskStore>();
			var subscriber = new ServerHeartbeatSubscriber(store.Object);

			Assert.AreEqual("server", subscriber.EventKey);
		}

		[Test]
		public void ServerHeartbeatSubscriber_RaiseEvent_TaskStore()
		{
			var store = new Mock<ITaskStore>();
			var subscriber = new ServerHeartbeatSubscriber(store.Object);

			subscriber.RaiseEvent();

			store.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.Once);
		}

		[Test]
		public void ServerHeartbeatSubscriber_RaiseEvent_Storage()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);
			var subscriber = new ServerHeartbeatSubscriber(store);

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.GetKeys(It.Is<StorageKey>(k => k.Key == "server")), Times.Once);
		}

		[Test]
		public void ServerHeartbeatSubscriber_RaiseEvent_PropagateServer()
		{
			var server = new ServerModel
			{
				Id = "1", 
				Heartbeat = DateTime.Now, 
				Name = "test"
			};

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(() => new[] {"key"});
			storage.Setup(exp => exp.Get<ServerModel>(It.IsAny<StorageKey>())).Returns(() => server);
			var store = new TaskStore(storage.Object);
			var subscriber = new ServerHeartbeatSubscriber(store);

			subscriber.RaiseEvent();

			Assert.AreSame(server, store.Servers.Single());
		}
	}
}
