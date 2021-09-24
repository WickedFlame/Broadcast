using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Server
{
	public class BroadcasterHeartbeatDispatcherTests
	{
		[Test]
		public void BroadcasterHeartbeatDispatcher_ctor()
		{
			var storage = new Mock<ITaskStore>();
			Assert.DoesNotThrow(() => new BroadcasterHeartbeatDispatcher(storage.Object, new Options()));
		}

		[Test]
		public void BroadcasterHeartbeatDispatcher_ctor_Null_Storage()
		{
			Assert.Throws<ArgumentNullException>(() => new BroadcasterHeartbeatDispatcher(null, new Options()));
		}

		[Test]
		public void BroadcasterHeartbeatDispatcher_ctor_Null_Options()
		{
			var storage = new Mock<ITaskStore>();
			Assert.Throws<ArgumentNullException>(() => new BroadcasterHeartbeatDispatcher(storage.Object, null));
		}

		[Test]
		public void BroadcasterHeartbeatDispatcher_Execute()
		{
			var options = new Options
			{
				ServerName = "BroadcasterHeartbeatDispatcher",
				HeartbeatInterval = 1
			};
			var context = new BroadcasterConterxt
			{
				IsRunning = true,
				Id = "1"
			};
			var storage = new Mock<ITaskStore>();
			storage.Setup(exp => exp.Storage(It.IsAny<Action<IStorage>>())).Callback(() => context.IsRunning = false);
			
			var dispatcher = new BroadcasterHeartbeatDispatcher(storage.Object, options);
			dispatcher.Execute(context);

			storage.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.Once);
		}

		[Test]
		public void BroadcasterHeartbeatDispatcher_Execute_SetStorage()
		{
			var options = new Options
			{
				ServerName = "BroadcasterHeartbeatDispatcher",
				HeartbeatInterval = 1
			};
			var context = new BroadcasterConterxt
			{
				IsRunning = true,
				Id = "1"
			};
			var storage = new Mock<IStorage>();

			var store = new TaskStore(storage.Object);
			storage.Setup(exp => exp.Set(It.IsAny<StorageKey>(), It.IsAny<ServerModel>())).Callback(() => context.IsRunning = false);

			var dispatcher = new BroadcasterHeartbeatDispatcher(store, options);
			dispatcher.Execute(context);

			storage.Verify(exp => exp.Set(It.Is<StorageKey>(k => k.Key == $"server:{options.ServerName}:{context.Id}"), It.Is<ServerModel>(s => s.Id == context.Id && s.Name == options.ServerName)), Times.Once);
		}

		[Test]
		public void BroadcasterHeartbeatDispatcher_Execute_PropagateEvent()
		{
			var options = new Options
			{
				ServerName = "BroadcasterHeartbeatDispatcher",
				HeartbeatInterval = 1
			};
			var context = new BroadcasterConterxt
			{
				IsRunning = true,
				Id = "1"
			};
			var storage = new Mock<IStorage>();

			var store = new TaskStore(storage.Object);
			storage.Setup(exp => exp.Set(It.IsAny<StorageKey>(), It.IsAny<ServerModel>())).Callback(() => context.IsRunning = false);

			var dispatcher = new BroadcasterHeartbeatDispatcher(store, options);
			dispatcher.Execute(context);

			storage.Verify(exp => exp.PropagateEvent(It.Is<StorageKey>(k => k.Key == $"server:{options.ServerName}:{context.Id}")), Times.Once);
		}
	}
}
