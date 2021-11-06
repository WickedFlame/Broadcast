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
			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			client.Object.Recurring("Id", () => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.Name == "Id" && t.State == TaskState.New && t.Time.Value.Seconds == 5)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_IsRecurring()
		{
			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			client.Object.Recurring("Id", () => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.IsRecurring)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_NoName()
		{
			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			client.Object.Recurring(null, () => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.IsRecurring && t.Name == "Debug.WriteLine")), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_ReturnedId()
		{
			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			var id = client.Object.Recurring(null, () => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Recurring_OverwriteExistingTask()
		{
			var existing = new RecurringTask {ReferenceId = "existing"};
			var store = new Mock<ITaskStore>();
			store.Setup(x => x.Storage(It.IsAny<Func<IStorage, RecurringTask>>())).Returns(() => existing);
			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => store.Object);

			var id = client.Object.Recurring(null, () => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.Id == id && t.Id == existing.ReferenceId)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Schedule_Id()
		{
			var client = new Mock<IBroadcastingClient>();

			var id = client.Object.Schedule(() => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Schedule_Time()
		{
			var client = new Mock<IBroadcastingClient>();

			client.Object.Schedule(() => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.Time.Value.Seconds == 5)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Schedule_NotRecurring()
		{
			var client = new Mock<IBroadcastingClient>();

			client.Object.Schedule(() => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(5));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => !t.IsRecurring)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Send_Id()
		{
			var client = new Mock<IBroadcastingClient>();

			var id = client.Object.Send(() => System.Diagnostics.Debug.WriteLine("Test"));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Send_NoTime()
		{
			var client = new Mock<IBroadcastingClient>();

			client.Object.Send(() => System.Diagnostics.Debug.WriteLine("Test"));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => t.Time == null)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_Send_NotRecurring()
		{
			var client = new Mock<IBroadcastingClient>();

			client.Object.Send(() => System.Diagnostics.Debug.WriteLine("Test"));

			client.Verify(exp => exp.Enqueue(It.Is<ITask>(t => !t.IsRecurring)), Times.Once);
		}

		[Test]
		public void BroadcastingClient_DeleteTask()
		{
			var store = new Mock<ITaskStore>();
			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => store.Object);

			client.Object.DeleteTask("id");

			store.Verify(exp => exp.Delete("id"), Times.Once);
		}

		[Test]
		public void BroadcastingClient_DeleteRecurringTask_NoExisting()
		{
			var store = new Mock<ITaskStore>();
			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => store.Object);

			client.Object.DeleteRecurringTask("id");

			store.Verify(exp => exp.Delete(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void BroadcastingClient_DeleteRecurringTask()
		{
			var existing = new RecurringTask { ReferenceId = "existing" };
			var store = new Mock<ITaskStore>();
			store.Setup(x => x.Storage(It.IsAny<Func<IStorage, RecurringTask>>())).Returns(() => existing);

			var client = new Mock<IBroadcastingClient>();
			client.Setup(exp => exp.Store).Returns(() => store.Object);

			client.Object.DeleteRecurringTask("id");

			store.Verify(exp => exp.Delete(existing.ReferenceId), Times.Once);
		}
	}
}
