using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Polaroider;
using StackExchange.Redis;

namespace Broadcast.Storage.Redis.Test
{
	public class RedisStorageTests
	{
		private Mock<IConnectionMultiplexer> _multiplexer;
		private Mock<IDatabase> _db;
		private Mock<ISubscriber> _subscriber;

		[SetUp]
		public void Setup()
		{
			_multiplexer = new Mock<IConnectionMultiplexer>();
			_db = new Mock<IDatabase>();
			var server = new Mock<IServer>();
			_subscriber = new Mock<ISubscriber>();
			_multiplexer.Setup(exp => exp.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_db.Object);
			_multiplexer.Setup(exp => exp.GetServer(It.IsAny<System.Net.EndPoint>(), null)).Returns(server.Object);
			_multiplexer.Setup(exp => exp.GetSubscriber(null)).Returns(_subscriber.Object);
		}

		[Test]
		public void RedisStorage_Ctor()
		{
			Assert.DoesNotThrow(() => new RedisStorage(_multiplexer.Object, new RedisStorageOptions()));
		}

		[Test]
		public void RedisStorage_Set()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.Set(new StorageKey("storage", "key"), "value");

			_db.Verify(exp => exp.HashSetAsync(It.IsAny<RedisKey>(), new[] { new HashEntry("value".GetType().Name, "value") }, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_Set_Publish()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.Set(new StorageKey("storage", "key"), "value");

			_db.Verify(exp => exp.PublishAsync("BroadcastTaskFetchChannel", "{broadcast}:key:storage", CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_Set_Object()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.Set(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });

			_db.Verify(exp => exp.HashSetAsync(It.IsAny<RedisKey>(), new[] { new HashEntry("Id", "1"), new HashEntry("Value", "one") }, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_Get()
		{
			_db.Setup(exp => exp.HashGetAll(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(() => new[]
			{
				new HashEntry("Value", "value"),
				new HashEntry("Id", "1")
			});

			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());

			Assert.AreEqual("value", storage.Get<string>(new StorageKey("storage", "key")));
		}

		[Test]
		public void RedisStorage_AddToList()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.AddToList(new StorageKey("storage", "key"), "value");

			_db.Verify(exp => exp.ListRightPushAsync(It.IsAny<RedisKey>(), It.Is<RedisValue>(v => v.ToString() == "value"), When.Always, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_AddToList_Multiple()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.AddToList(new StorageKey("storage", "key"), "one");
			storage.AddToList(new StorageKey("storage", "key"), "two");

			_db.Verify(exp => exp.ListRightPushAsync(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key:storage"), It.IsAny<RedisValue>(), When.Always, CommandFlags.None), Times.Exactly(2));

			_db.Verify(exp => exp.ListRightPushAsync(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key:storage"), It.Is<RedisValue>(v => v.ToString() == "one"), When.Always, CommandFlags.None), Times.Once);
			_db.Verify(exp => exp.ListRightPushAsync(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key:storage"), It.Is<RedisValue>(v => v.ToString() == "two"), When.Always, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_Delete_Item()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.Delete(new StorageKey("storage", "key"));

			_db.Verify(exp => exp.KeyDeleteAsync(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key:storage"), CommandFlags.None), Times.Once);
		}







		[Test]
		public void RedisStorage_RegisterSubscription()
		{
			var subscription = new Mock<ISubscription>();

			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			Assert.DoesNotThrow(() => storage.RegisterSubscription(subscription.Object));
		}


		[Test]
		public void RedisStorage_RegisterSubscription_Verify()
		{
			var subscription = new Mock<ISubscription>();
			subscription.Setup(exp => exp.EventKey).Returns(() => "key");

			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.RegisterSubscription(subscription.Object);

			Assert.AreSame(subscription.Object, storage.Subscription.Subscriptions.Single());
		}

		[Test]
		public void RedisStorage_Set_Subscription_DifferentKey()
		{
			var subscription = new Mock<ISubscription>();
			subscription.Setup(exp => exp.EventKey).Returns(() => "different");

			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.RegisterSubscription(subscription.Object);

			storage.Set(new StorageKey("key:one"), "value");

			subscription.Verify(exp => exp.RaiseEvent(), Times.Never);
		}


		[Test]
		public void RedisStorage_Update()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.Set(new StorageKey("storage", "key"), "value");
			storage.Set(new StorageKey("storage", "key"), "reset");

			_db.Verify(exp => exp.HashSetAsync(It.IsAny<RedisKey>(), new[] { new HashEntry("value".GetType().Name, "value") }, CommandFlags.None), Times.Once);
			_db.Verify(exp => exp.HashSetAsync(It.IsAny<RedisKey>(), new[] { new HashEntry("value".GetType().Name, "reset") }, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_List_Objects()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.AddToList(new StorageKey("storage", "key"), "one");
			storage.AddToList(new StorageKey("storage", "key"), "two");

			_db.Verify(exp => exp.ListRightPushAsync(It.IsAny<RedisKey>(), It.Is<RedisValue>(v => v.ToString() == "one"), When.Always, CommandFlags.None), Times.Once);
			_db.Verify(exp => exp.ListRightPushAsync(It.IsAny<RedisKey>(), It.Is<RedisValue>(v => v.ToString() == "two"), When.Always, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_Item_Object()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.Set(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });

			_db.Verify(exp => exp.HashSetAsync(It.IsAny<RedisKey>(), new[] { new HashEntry("Id", "1"), new HashEntry("Value", "one") }, CommandFlags.None), Times.Once);

		}

		[Test]
		public void RedisStorage_RemoveRange()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());

			storage.RemoveRangeFromList(new StorageKey("storage", "key"), 3);

			_db.Verify(exp => exp.ListLeftPopAsync(It.IsAny<RedisKey>(), CommandFlags.None), Times.Exactly(3));
		}

		[Test]
		public void RedisStorage_RemoveFromList()
		{
			var item = "two";

			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			
			storage.RemoveFromList(new StorageKey("storage", "key"), item);

			_db.Verify(exp => exp.ListRemoveAsync(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key:storage"), It.Is<RedisValue>(v => v.ToString() == item), 0, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_GetList()
		{
			_db.Setup(exp => exp.ListRange(It.IsAny<RedisKey>(), 0, -1, CommandFlags.None)).Returns(() => new[]
			{
				new RedisValue("one"),
				new RedisValue("two")
			});

			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());

			var items = storage.GetList(new StorageKey("storage", "key"));

			Assert.AreEqual("one", items.First());
			Assert.AreEqual("two", items.Last());
		}

		[Test]
		public void RedisStorage_Get_Invalid()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.AddToList(new StorageKey("storage", "key"), "one");

			Assert.IsNull(storage.Get<StorageModel>(new StorageKey("storage", "key")));
		}

		[Test]
		public void RedisStorage_GetList_Invalid()
		{
			_db.Setup(exp => exp.ListRange(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key:storage"), 0, -1, CommandFlags.None)).Returns(() => new[]
			{
				new RedisValue("one"),
				new RedisValue("two")
			});

			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());

			Assert.IsEmpty(storage.GetList(new StorageKey("storage", "invalid")));
			_db.Verify(exp => exp.ListRange(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:invalid:storage"), 0, -1, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_SetValues()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			_db.Setup(exp => exp.HashGetAll(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key1"), CommandFlags.None)).Returns(new[] {new HashEntry("one", "1"), new HashEntry("two", "2")});

			var obj = storage.Get<DataObject>(new StorageKey("key1"));

			//TODO: should be int instead of string
			Assert.AreEqual(1, obj["one"]);
			Assert.AreEqual(2, obj["two"]);
		}

		[Test]
		public void RedisStorage_SetValues_Overwrite()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			storage.SetValues(new StorageKey("key1"), new DataObject
			{
				{"one", 1},
				{"two", 2}
			});

			storage.SetValues(new StorageKey("key1"), new DataObject
			{
				{"one", 3}
			});

			_db.Verify(exp => exp.HashSetAsync(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key1"), It.IsAny<HashEntry[]>(), CommandFlags.None), Times.Exactly(2));
		}


		[Test]
		public void RedisStorage_TryFetchNext()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			_db.Setup(exp => exp.ListRightPopLeftPush(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key1"), It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key2"), CommandFlags.None)).Returns("key moved");

			storage.TryFetchNext(new StorageKey("key1"), new StorageKey("key2"), out var item);

			Assert.AreEqual("key moved", item);
		}

		[Test]
		public void RedisStorage_TryFetchNext_True()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			_db.Setup(exp => exp.ListRightPopLeftPush(It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key1"), It.Is<RedisKey>(k => k.ToString() == "{broadcast}:key2"), CommandFlags.None)).Returns("key moved");

			Assert.IsTrue(storage.TryFetchNext(new StorageKey("key1"), new StorageKey("key2"), out var item));
		}

		[Test]
		public void RedisStorage_TryFetchNext_NoItem()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			//_db.Setup(exp => exp.ListRightPopLeftPush(It.Is<RedisKey>(k => k.ToString() == "key1"), It.Is<RedisKey>(k => k.ToString() == "key2"), CommandFlags.None)).Returns("key moved");

			storage.TryFetchNext(new StorageKey("key1"), new StorageKey("key2"), out var item);

			Assert.IsNull(item);
		}

		[Test]
		public void RedisStorage_TryFetchNext_False()
		{
			var storage = new RedisStorage(_multiplexer.Object, new RedisStorageOptions());
			//_db.Setup(exp => exp.ListRightPopLeftPush(It.Is<RedisKey>(k => k.ToString() == "key1"), It.Is<RedisKey>(k => k.ToString() == "key2"), CommandFlags.None)).Returns("key moved");

			Assert.IsFalse(storage.TryFetchNext(new StorageKey("key1"), new StorageKey("key2"), out var item));
		}
		

		public class StorageModel
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}