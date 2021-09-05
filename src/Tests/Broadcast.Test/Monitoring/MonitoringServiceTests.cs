using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Monitoring;
using Broadcast.Server;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Test.Monitoring
{
	public class MonitoringServiceTests
	{
		[Test]
		public void MonitoringService_ctor()
		{
			var store = new Mock<ITaskStore>();
			Assert.DoesNotThrow(() => new MonitoringService(store.Object));
		}

		[Test]
		public void MonitoringService_ctor_NoStore()
		{
			Assert.Throws<ArgumentNullException>(() => new MonitoringService(null));
		}

		[Test]
		public void MonitoringService_GetServers_Count()
		{
			var store = new Mock<ITaskStore>();
			store.Setup(exp => exp.Servers).Returns(() => new List<ServerModel>
			{
				new ServerModel(),
				new ServerModel()
			});
			var monitor = new MonitoringService(store.Object);

			var servers = monitor.GetServers();

			Assert.AreEqual(2, servers.Count());
		}

		[Test]
		public void MonitoringService_GetServers_Match()
		{
			var model = new ServerModel
			{
				Id = "id",
				Heartbeat = DateTime.Now,
				Name = "testserver"
			};
			var store = new Mock<ITaskStore>();
			store.Setup(exp => exp.Servers).Returns(() => new List<ServerModel> { model });
			var monitor = new MonitoringService(store.Object);

			var servers = monitor.GetServers();

			Assert.AreEqual(model.Id, servers.First().Id);
			Assert.AreEqual(model.Name, servers.First().Name);
			Assert.AreEqual(model.Heartbeat, servers.First().Heartbeat);
		}

		[Test]
		public void MonitoringService_GetAllTasks_Count()
		{
			var tasks = new List<ITask>
			{
				new Mock<ITask>().Object,
				new Mock<ITask>().Object
			};
			var store = new Mock<ITaskStore>();
			store.Setup(exp => exp.GetEnumerator()).Returns(() => tasks.GetEnumerator());

			var monitor = new MonitoringService(store.Object);

			var servers = monitor.GetAllTasks();

			Assert.AreEqual(2, servers.Count());
		}

		[Test]
		public void MonitoringService_GetRecurringTasks()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(() => new List<string> {"test:1", "test:2"});
			storage.Setup(exp => exp.Get<RecurringTask>(It.IsAny<StorageKey>())).Returns<StorageKey>(s => new RecurringTask{ReferenceId = s.ToString()});

			var store = new TaskStore(storage.Object);

			var monitor = new MonitoringService(store);

			var servers = monitor.GetRecurringTasks();

			Assert.AreEqual(2, servers.Count());
		}

		[Test]
		public void MonitoringService_GetRecurringTasks_Match()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(() => new List<string> { "test:1", "test:2" });
			storage.Setup(exp => exp.Get<RecurringTask>(It.IsAny<StorageKey>())).Returns<StorageKey>(s => new RecurringTask
			{
				ReferenceId = s.ToString(),
				Name = s.ToString(),
				Interval = TimeSpan.FromSeconds(10)
			});

			var store = new TaskStore(storage.Object);

			var monitor = new MonitoringService(store);

			monitor.GetRecurringTasks().MatchSnapshot();
		}
	}
}
