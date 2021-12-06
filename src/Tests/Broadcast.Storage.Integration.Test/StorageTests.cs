using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Storage.Integration.Test
{
	[Category("Integration")]
	public partial class StorageTests
	{
		[Test]
		public void Storage_Ctor()
		{
			Assert.DoesNotThrow(() =>
			{
				try
				{
					BuildStorage();
				}
				catch (IgnoreException)
				{
					// test is ignored
					// so do nothing
				}
				catch (Exception)
				{
					throw;
				}
			});
		}

		[Test]
		public void Storage_Set()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "set"), "value");

            Assert.IsNotNull(storage.Get<string>(new StorageKey("storage", "set")));
        }

		[Test]
		public void Storage_Set_Object()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "setobject"), new StorageModel { Id = 1, Value = "one" });

			storage.Get<StorageModel>(new StorageKey("storage", "setobject")).MatchSnapshot();
		}

		[Test]
		public void Storage_PropagateEvent_Subscription()
		{
			var subscription = new Mock<ISubscription>();
			subscription.Setup(exp => exp.EventKey).Returns(() => "subscription");

			var storage = BuildStorage();
			storage.RegisterSubscription(subscription.Object);

			storage.PropagateEvent(new StorageKey("subscription:one"));

			subscription.Verify(exp => exp.RaiseEvent(), Times.Once);
		}

		[Test]
		public void Storage_PropagateEvent_Subscription_CaseInsensitive()
		{
			var subscription = new Mock<ISubscription>();
			subscription.Setup(exp => exp.EventKey).Returns(() => "KEY");

			var storage = BuildStorage();
			storage.RegisterSubscription(subscription.Object);

			storage.PropagateEvent(new StorageKey("kEy:CaseInsensitive"));

			subscription.Verify(exp => exp.RaiseEvent(), Times.Once);
		}

		[Test]
		public void Storage_Set_Subscription_DifferentKey()
		{
			var subscription = new Mock<ISubscription>();
			subscription.Setup(exp => exp.EventKey).Returns(() => "different");

			var storage = BuildStorage();
			storage.RegisterSubscription(subscription.Object);

			storage.Set(new StorageKey("key:otherKey"), "value");

			subscription.Verify(exp => exp.RaiseEvent(), Times.Never);
		}

		[Test]
		public void Storage_AddToList()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "addtolist"), "value");

            Assert.AreEqual(1, storage.GetList(new StorageKey("storage", "addtolist")).Count());
		}

		[Test]
		public void Storage_AddToList_Multiple()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "addtolist_multy"), "one");
			storage.AddToList(new StorageKey("storage", "addtolist_multy"), "two");

            Assert.AreEqual(2, storage.GetList(new StorageKey("storage", "addtolist_multy")).Count());
        }

		[Test]
		public void Storage_Get()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "get"), "value");

			Assert.AreEqual("value", storage.Get<string>(new StorageKey("storage", "get")));
		}

		[Test]
		public void Storage_Update()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "update"), "value");
			storage.Set(new StorageKey("storage", "update"), "reset");

			Assert.AreEqual("reset", storage.Get<string>(new StorageKey("storage", "update")));
		}

		[Test]
		public void Storage_List_Objects()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "list_objects"), "one");
			storage.AddToList(new StorageKey("storage", "list_objects"), "two");

			var items = storage.GetList(new StorageKey("storage", "list_objects"));
			items.MatchSnapshot();
		}

		[Test]
		public void Storage_Item_Object()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "item_object"), new StorageModel { Id = 1, Value = "one" });

			var item = storage.Get<StorageModel>(new StorageKey("storage", "item_object"));
			item.MatchSnapshot();
		}

		[Test]
		public void Storage_Delete_List()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "delete_list"), "value");
			storage.AddToList(new StorageKey("storage", "delete_list"), "reset");

			Assert.AreEqual(2, storage.GetList(new StorageKey("storage", "delete_list")).Count());

			storage.Delete(new StorageKey("storage", "delete_list"));

			Assert.IsEmpty(storage.GetList(new StorageKey("storage", "delete_list")));
		}

		[Test]
		public void Storage_Delete_Item()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "delete_item"), "value");

			Assert.IsNotNull(storage.Get<string>(new StorageKey("storage", "delete_item")));

			storage.Delete(new StorageKey("storage", "delete_item"));

			Assert.IsNull(storage.Get<string>(new StorageKey("storage", "delete_item")));
		}

		[Test]
		public void Storage_List_Distinct()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "list_distinct"), "one");
			storage.AddToList(new StorageKey("storage", "list_distinct"), "one");

			Assert.That(storage.GetList(new StorageKey("storage", "list_distinct")).Count(), Is.EqualTo(1));
		}

		[Test]
		public void Storage_RemoveRange()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "remove_range"), "one");
			storage.AddToList(new StorageKey("storage", "remove_range"), "two");

			Assert.AreEqual(2, storage.GetList(new StorageKey("storage", "remove_range")).Count());

			storage.RemoveRangeFromList(new StorageKey("storage", "remove_range"), 3);

			Assert.IsEmpty(storage.GetList(new StorageKey("storage", "remove_range")));
		}

		[Test]
		public void Storage_RemoveFromList()
		{
			var item = "two";

			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "remove_from_list"), "one");
			storage.AddToList(new StorageKey("storage", "remove_from_list"), item);

			storage.RemoveFromList(new StorageKey("storage", "remove_from_list"), item);

			Assert.IsTrue(storage.GetList(new StorageKey("storage", "remove_from_list")).All(i => i != item));
		}

		[Test]
		public void Storage_Get_Invalid()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("storage", "get_invalid"), "one");

			Assert.IsNull(storage.Get<StorageModel>(new StorageKey("storage", "get_invalid")));
		}

		[Test]
		public void Storage_GetList_Invalid()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "getlist_invalid"), new StorageModel { Id = 1, Value = "one" });

			Assert.IsEmpty(storage.GetList(new StorageKey("storage", "getlist_invalid")));
		}

		[Test]
		public void Storage_Get_Invalid_Type()
		{
			var storage = BuildStorage();
			storage.Set(new StorageKey("storage", "get_invalid_type"), new StorageModel { Id = 1, Value = "one" });

			Assert.IsNotNull(storage.Get<InmemoryStorage>(new StorageKey("storage", "get_invalid_type")));
		}

		[Test]
		public void Storage_RegisterSubscription()
		{
			var subscription = new Mock<ISubscription>();

			var storage = BuildStorage();
			Assert.DoesNotThrow(() => storage.RegisterSubscription(subscription.Object));
		}

		[Test]
		public void Storage_TryFetchNext_List()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("fetch_key1"), "one");

			storage.TryFetchNext(new StorageKey("fetch_key1"), new StorageKey("fetch_key2"), out var item);

			Assert.IsEmpty(storage.GetList(new StorageKey("fetch_key1")));

			Assert.That(storage.GetList(new StorageKey("fetch_key2")).Single(), Is.EqualTo(item));
		}

		[Test]
		public void Storage_TryFetchNext_List_True()
		{
			var storage = BuildStorage();
			storage.AddToList(new StorageKey("fetch_list_key1"), "one");

			Assert.IsTrue(storage.TryFetchNext(new StorageKey("fetch_list_key1"), new StorageKey("fetch_list_key2"), out var item));
		}

		[Test]
		public void Storage_SetValues()
		{
			var storage = BuildStorage();
			storage.SetValues(new StorageKey("set_values"), new DataObject
			{
				{"one", 1},
				{"two", 2}
			});

			var obj = storage.Get<DataObject>(new StorageKey("set_values"));

			Assert.AreEqual(1, obj["one"]);
			Assert.AreEqual(2, obj["two"]);
		}

		[Test]
		public void Storage_SetValues_Overwrite()
		{
			var storage = BuildStorage();
			storage.SetValues(new StorageKey("set_values_override"), new DataObject
			{
				{"one", 1},
				{"two", 2}
			});

			storage.SetValues(new StorageKey("set_values_override"), new DataObject
			{
				{"one", 3}
			});

			var obj = storage.Get<DataObject>(new StorageKey("set_values_override"));

			Assert.AreEqual(3, obj["one"]);
			Assert.AreEqual(2, obj["two"]);
		}

		public class StorageModel
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}
