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
		public void DispatcherStorage_InitialList()
		{
			var storage = new DispatcherStorage();
			Assert.IsEmpty(storage);
		}

		[Test]
		public void DispatcherStorage_Add()
		{
			var dispatcher = new Mock<IDispatcher>();
			var storage = new DispatcherStorage();
			storage.Add("id", new []
			{
				dispatcher.Object
			});

			Assert.AreSame(dispatcher.Object, storage.Single());
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

			Assert.AreEqual(4, storage.Count());
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

			Assert.AreEqual(2, storage.Count());
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

			Assert.AreSame(dispatcher.Object, storage.Single());
		}

		[Test]
		public void DispatcherStorage_Remove_Unadded()
		{
			var storage = new DispatcherStorage();
			Assert.DoesNotThrow(() => storage.Remove("id1"));
		}
	}
}
