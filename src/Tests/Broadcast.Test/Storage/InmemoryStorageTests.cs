using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Storage;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Test.Storage
{
	public class InmemoryStorageTests
	{
		[Test]
		public void InmemoryStorage_Ctor()
		{
			var storage = new InmemoryStorage();
		}

		[Test]
		public void InmemoryStorage_Set()
		{
			var storage = new InmemoryStorage();
			storage.Set(new StorageKey("storage", "key"), "value");
		}

		[Test]
		public void InmemoryStorage_AddToList()
		{
			var storage = new InmemoryStorage();
			storage.AddToList(new StorageKey("storage", "key"), "value");
		}

		[Test]
		public void InmemoryStorage_AddToList_Multiple()
		{
			var storage = new InmemoryStorage();
			storage.AddToList(new StorageKey("storage", "key"), "one");
			storage.AddToList(new StorageKey("storage", "key"), "two");
		}

		[Test]
		public void InmemoryStorage_Get()
		{
			var storage = new InmemoryStorage();
			storage.Set(new StorageKey("storage", "key"), "value");

			Assert.AreEqual("value", storage.Get<string>(new StorageKey("storage", "key")));
		}

		[Test]
		public void InmemoryStorage_Update()
		{
			var storage = new InmemoryStorage();
			storage.Set(new StorageKey("storage", "key"), "value");
			storage.Set(new StorageKey("storage", "key"), "reset");

			Assert.AreEqual("reset", storage.Get<string>(new StorageKey("storage", "key")));
		}

		[Test]
		public void InmemoryStorage_List_Objects()
		{
			var storage = new InmemoryStorage();
			storage.AddToList(new StorageKey("storage", "key"), new StorageModel {Id = 1, Value = "one"});
			storage.AddToList(new StorageKey("storage", "key"), new StorageModel { Id = 2, Value = "two" });

			var items = storage.GetList<StorageModel>(new StorageKey("storage", "key"));
			items.MatchSnapshot();
		}

		[Test]
		public void InmemoryStorage_Item_Object()
		{
			var storage = new InmemoryStorage();
			storage.Set(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });

			var item = storage.Get<StorageModel>(new StorageKey("storage", "key"));
			item.MatchSnapshot();
		}

		[Test]
		public void InmemoryStorage_Delete_List()
		{
			var storage = new InmemoryStorage();
			storage.AddToList(new StorageKey("storage", "key"), "value");
			storage.AddToList(new StorageKey("storage", "key"), "reset");

			Assert.AreEqual(2, storage.GetList<string>(new StorageKey("storage", "key")).Count());

			storage.Delete(new StorageKey("storage", "key"));

			Assert.IsEmpty(storage.GetList<string>(new StorageKey("storage", "key")));
		}

		[Test]
		public void InmemoryStorage_Delete_Item()
		{
			var storage = new InmemoryStorage();
			storage.Set(new StorageKey("storage", "key"), "value");

			Assert.IsNotNull(storage.Get<string>(new StorageKey("storage", "key")));

			storage.Delete(new StorageKey("storage", "key"));

			Assert.IsNull(storage.Get<string>(new StorageKey("storage", "key")));
		}

		[Test]
		public void InmemoryStorage_RemoveRange()
		{
			var storage = new InmemoryStorage();
			storage.AddToList(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });
			storage.AddToList(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });

			Assert.AreEqual(2, storage.GetList<StorageModel>(new StorageKey("storage", "key")).Count());

			storage.RemoveRangeFromList(new StorageKey("storage", "key"), 3);

			Assert.IsEmpty(storage.GetList<StorageModel>(new StorageKey("storage", "key")));
		}

		[Test]
		public void InmemoryStorage_Get_Invalid()
		{
			var storage = new InmemoryStorage();
			storage.AddToList(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });

			Assert.IsNull(storage.Get<StorageModel>(new StorageKey("storage", "key")));
		}

		[Test]
		public void InmemoryStorage_GetList_Invalid()
		{
			var storage = new InmemoryStorage();
			storage.Set(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });

			Assert.IsEmpty(storage.GetList<StorageModel>(new StorageKey("storage", "key")));
		}

		public class StorageModel
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}
