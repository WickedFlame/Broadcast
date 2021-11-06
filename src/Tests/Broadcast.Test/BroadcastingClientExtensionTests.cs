using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Test
{
	public class BroadcastingClientExtensionTests
	{
		[Test]
		public void BroadcastingClient_Recurring()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.Recurring("Id", () => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Name == "Id" && t.State == TaskState.New && t.Time.Value.Seconds == 5)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_IsRecurring()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.Recurring("Id", () => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.IsRecurring)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_NoName()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.Recurring(null, () => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.IsRecurring && t.Name == "Trace.WriteLine")), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_ReturnedId()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			var id = client.Recurring(null, () => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_OverwriteExistingTask()
		{
			var existing = new RecurringTask {ReferenceId = "existing"};
			var store = new Mock<ITaskStore>();
			store.Setup(x => x.Storage(It.IsAny<Func<IStorage, RecurringTask>>())).Returns(() => existing);
			var client = new BroadcastingClient(store.Object);

			var id = client.Recurring(null, () => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Id == id && t.Id == existing.ReferenceId)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Schedule_Id()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			var id = client.Schedule(() => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Schedule_Time()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.Schedule(() => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Time.Value.Seconds == 5)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Schedule_NotRecurring()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.Schedule(() => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(5));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => !t.IsRecurring)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Send_Id()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			var id = client.Send(() => System.Diagnostics.Trace.WriteLine("Test"));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Send_NoTime()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.Send(() => System.Diagnostics.Trace.WriteLine("Test"));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Time == null)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Send_NotRecurring()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.Send(() => System.Diagnostics.Trace.WriteLine("Test"));

			store.Verify(exp => exp.Add(It.Is<ITask>(t => !t.IsRecurring)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_DeleteTask()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.DeleteTask("id");

			store.Verify(exp => exp.Delete("id"), Times.Once);
		}

		[Test]
		public void BroadcastingClient_DeleteRecurringTask_NoExisting()
		{
			var store = new Mock<ITaskStore>();
			var client = new BroadcastingClient(store.Object);

			client.DeleteRecurringTask("id");

			store.Verify(exp => exp.Delete(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void BroadcastingClient_DeleteRecurringTask()
		{
			var existing = new RecurringTask { ReferenceId = "existing" };
			var store = new Mock<ITaskStore>();
			store.Setup(x => x.Storage(It.IsAny<Func<IStorage, RecurringTask>>())).Returns(() => existing);

			var client = new BroadcastingClient(store.Object);

			client.DeleteRecurringTask("id");

			store.Verify(exp => exp.Delete(existing.ReferenceId), Times.Once);
		}
	}
}
