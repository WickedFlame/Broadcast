using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Composition;
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
		public void MonitoringService_GetAllTasks_Properties()
		{
			var tasks = new List<ITask>
			{
				TaskFactory.CreateTask(()=> System.Diagnostics.Trace.WriteLine("MonitoringService"))
			};
			var store = new Mock<ITaskStore>();
			store.Setup(exp => exp.GetEnumerator()).Returns(() => tasks.GetEnumerator());
			store.Setup(exp => exp.Storage<DataObject>(It.IsAny<Func<IStorage, DataObject>>())).Returns(() => new DataObject
			{
				{"Server", "TestServer"}
			});

			var monitor = new MonitoringService(store.Object);

			monitor.GetAllTasks().MatchSnapshot(SnapshotOptions.Create(o => o.AddDirective(d => d.ReplaceGuid())));
		}

		[Test]
		public void MonitoringService_GetAllTasks_NoProperties()
		{
			var tasks = new List<ITask>
			{
				TaskFactory.CreateTask(()=> System.Diagnostics.Trace.WriteLine("MonitoringService"))
			};
			var store = new Mock<ITaskStore>();
			store.Setup(exp => exp.GetEnumerator()).Returns(() => tasks.GetEnumerator());
			store.Setup(exp => exp.Storage<DataObject>(It.IsAny<Func<IStorage, DataObject>>())).Returns(() => null);

			var monitor = new MonitoringService(store.Object);

			monitor.GetAllTasks().MatchSnapshot(SnapshotOptions.Create(o => o.AddDirective(d => d.ReplaceGuid())));
		}

		[Test]
		public void MonitoringService_GetRecurringTasks()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(() => new List<string> {"test:1", "test:2"});
			storage.Setup(exp => exp.Get<RecurringTask>(It.IsAny<StorageKey>())).Returns<StorageKey>(s => new RecurringTask{ReferenceId = s.ToString()});

			var store = new TaskStore(storage.Object);

			var monitor = new MonitoringService(store);

			var tasks = monitor.GetRecurringTasks();

			Assert.AreEqual(2, tasks.Count());
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
