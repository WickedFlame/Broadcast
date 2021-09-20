using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Dashboard.Dispatchers.Models;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Dashboard.Test
{
	public class StorageItemServiceTests
	{
		[Test]
		public void StorageItemService_ctor()
		{
			Assert.DoesNotThrow(() => new StorageItemService(new Mock<ITaskStore>().Object));
		}

		[Test]
		public void StorageItemService_ctor_NullStore()
		{
			Assert.Throws<ArgumentNullException>(() => new StorageItemService(null));
		}

		[Test]
		public void StorageItemService_GetTask()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.Is<StorageKey>(k => k.Key.EndsWith("task:E09105CE-8A21-4C51-B2A2-5E6A6B63889A")))).Returns(() => new DataObject
			{
				{"Id", "0fcd82f9-d797-420e-8d7a-2e392735a677"},
				{"Name", "Trace.WriteLine"},
				{"State", "Processed"},
				{"Type", "System.Diagnostics.Trace, System.Diagnostics.TraceSource"},
				{"IsRecurring", "True"},
				{"Time", "16/09/2021 00:00:20"},
				{"StateChanges:New", "2021/12/21T12:12:12"},
				{"StateChanges:Queued", "2021/12/21T12:12:12"},
				{"StateChanges:Dequeued", "2021/12/21T12:12:12"},
				{"StateChanges:InProcess", "2021/12/21T12:12:12"},
				{"StateChanges:Processed", "2021/12/21T12:12:12"},
				{"Method", "WriteLine"},
				{"ArgsType:0","System.String, System.Private.CoreLib"},
				{"ArgsValue:0","Broadcast Server task set from Startup"},
				{"Server","OCH-SB-CWA-N"}
			});

			storage.Setup(exp => exp.Get<DataObject>(It.Is<StorageKey>(k => k.Key.EndsWith("tasks:values:E09105CE-8A21-4C51-B2A2-5E6A6B63889A")))).Returns(() => new DataObject
			{
				{"QueuedAt","2021/12/21T12:12:12"},
				{"DequeuedAt","2021/12/21T12:12:12"},
				{"InProcessAt","2021/12/21T12:12:12"},
				{"State","Processed"},
				{"ProcessedAt","2021/12/21T12:12:12"},
				{"ExecutionTime","25"},
				{"ExecutedAt","2021/12/21T12:12:12"}
			});

			var store = new TaskStore(storage.Object);

			var service = new StorageItemService(store);

			var task = service.GetTask("E09105CE-8A21-4C51-B2A2-5E6A6B63889A");

			task.MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes()));
		}

		[Test]
		public void StorageItemService_GetTask_Simple()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.Is<StorageKey>(k => k.Key.EndsWith("task:E09105CE-8A21-4C51-B2A2-5E6A6B63889A")))).Returns(() => new DataObject
			{
				{"Id", "0fcd82f9-d797-420e-8d7a-2e392735a677"},
				{"Name", "Trace.WriteLine"},
				{"State", "New"},
				{"Type", "System.Diagnostics.Trace, System.Diagnostics.TraceSource"},
				{"IsRecurring", "True"},
				{"Time", "16/09/2021 00:00:20"},
				{"StateChanges:New", "2021/12/21T12:12:12"},
				{"Method", "WriteLine"},
				{"ArgsType:0","System.String, System.Private.CoreLib"},
				{"ArgsValue:0","Broadcast Server task set from Startup"}
			});

			var store = new TaskStore(storage.Object);

			var service = new StorageItemService(store);

			var task = service.GetTask("E09105CE-8A21-4C51-B2A2-5E6A6B63889A");

			task.MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes()));
		}

		[Test]
		public void StorageItemService_GetTask_NoData()
		{
			var storage = new Mock<IStorage>();

			var store = new TaskStore(storage.Object);

			var service = new StorageItemService(store);

			var task = service.GetTask("E09105CE-8A21-4C51-B2A2-5E6A6B63889A");

			Assert.IsNull(task);
		}



		[Test]
		public void StorageItemService_GetServer()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(new List<string>
			{
				"server:server2:6CA24559-DE30-4EF2-9F02-2595FC9D6C7F",
				"server:server1:E09105CE-8A21-4C51-B2A2-5E6A6B63889A"
			});
			storage.Setup(exp => exp.Get<DataObject>(It.Is<StorageKey>(k => k.Key.EndsWith("server:server1:E09105CE-8A21-4C51-B2A2-5E6A6B63889A")))).Returns(() => new DataObject
			{
				{"Id", "E09105CE-8A21-4C51-B2A2-5E6A6B63889A"},
				{"Name", "server1"},
				{"Heartbeat", "2021/12/21T12:12:12"}
			});

			var store = new TaskStore(storage.Object);

			var service = new StorageItemService(store);

			var task = service.GetServer("E09105CE-8A21-4C51-B2A2-5E6A6B63889A");

			task.MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes()));
		}


		[Test]
		public void StorageItemService_GetServer_NoData()
		{
			var storage = new Mock<IStorage>();

			var store = new TaskStore(storage.Object);

			var service = new StorageItemService(store);

			var data = service.GetServer("E09105CE-8A21-4C51-B2A2-5E6A6B63889A");

			Assert.IsNull(data);
		}
	}
}
