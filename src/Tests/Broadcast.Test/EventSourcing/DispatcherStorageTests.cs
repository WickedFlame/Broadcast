using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.EventSourcing;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class DispatcherStorageTests
	{
		[Test]
		public void DispatcherStorage_ctor()
		{
			Assert.DoesNotThrow(() => new DispatcherStorage());
		}

		[Test]
		public void DispatcherStorage_GetNext_Empty()
		{
			var storage = new DispatcherStorage();
			Assert.IsEmpty(storage.GetNext());
		}

		[Test]
		public void DispatcherStorage_Add()
		{
			var storage = new DispatcherStorage();
			var set = new[]
			{
				new Mock<IDispatcher>().Object
			};
			storage.Add("id", set);

			// DispatcherStorage creates a copy of the colleceion so we can't compare the collection but only the items
			Assert.AreSame(set.Single(), storage.GetNext().Single());
		}

		[Test]
		public void DispatcherStorage_Add_Multiple()
		{
			var storage = new DispatcherStorage();
			storage.Add("id1", new[]
			{
				new Mock<IDispatcher>().Object,
				new Mock<IDispatcher>().Object
			});

			storage.Add("id2", new[]
			{
				new Mock<IDispatcher>().Object,
				new Mock<IDispatcher>().Object
			});

			Assert.AreEqual(2, storage.Count());
		}

		[Test]
		public void DispatcherStorage_AddOverwrite()
		{
			var storage = new DispatcherStorage();
			storage.Add("id", new[]
			{
				new Mock<IDispatcher>().Object,
				new Mock<IDispatcher>().Object
			});
			//overwrite with same key
			storage.Add("id", new[]
			{
				new Mock<IDispatcher>().Object,
				new Mock<IDispatcher>().Object
			});

			Assert.AreEqual(1, storage.Count());
		}

		[Test]
		public void DispatcherStorage_Remove()
		{
			var storage = new DispatcherStorage();
			storage.Add("id1", new[]
			{
				new Mock<IDispatcher>().Object,
				new Mock<IDispatcher>().Object
			});

			var dispatcher = new Mock<IDispatcher>();
			storage.Add("id2", new[]
			{
				dispatcher.Object
			});

			storage.Remove("id1");

			Assert.AreSame(dispatcher.Object, storage.GetNext().Single());
		}

		[Test]
		public void DispatcherStorage_GetNext_Multiple()
		{
			var storage = new DispatcherStorage();
			var firstSet = new[]
			{
				new Mock<IDispatcher>().Object
			};
			storage.Add("id1", firstSet);

			var secondSet = new[]
			{
				new Mock<IDispatcher>().Object
			};
			storage.Add("id2", secondSet);

			// get first
			Assert.AreSame(firstSet.Single(), storage.GetNext().Single());
			// get next
			Assert.AreSame(secondSet.Single(), storage.GetNext().Single());
			// move to first again
			Assert.AreSame(firstSet.Single(), storage.GetNext().Single());
		}

		[Test]
		public void DispatcherStorage_GetNext_NamedQueue()
		{
			var storage = new DispatcherStorage();
			var firstSet = new[]
			{
				new Mock<IDispatcher>().Object
			};
			storage.Add("id1", firstSet);

			var secondSet = new[]
			{
				new Mock<IDispatcher>().Object
			};
			storage.Add("id2", secondSet);

			// get first
			Assert.AreSame(firstSet.Single(), storage.GetNext("id1").Single());
			// get first
			Assert.AreSame(secondSet.Single(), storage.GetNext("id2").Single());
		}

		[Test]
		public void DispatcherStorage_GetNext_NamedQueue_NotRegisteredQueue()
		{
			var storage = new DispatcherStorage();
			var firstSet = new[]
			{
				new Mock<IDispatcher>().Object
			};
			storage.Add("id1", firstSet);

			var secondSet = new[]
			{
				new Mock<IDispatcher>().Object
			};
			storage.Add("id2", secondSet);

			// if the queue does not exist we use round robin to get the next queue
			var queue = storage.GetNext("unregistered").Single();
			Assert.That(queue, Is.SameAs(firstSet.Single()).Or.SameAs(secondSet.Single()));
		}

		[Test]
		public void DispatcherStorage_Remove_Unadded()
		{
			var storage = new DispatcherStorage();
			Assert.DoesNotThrow(() => storage.Remove("id1"));
		}
	}
}
