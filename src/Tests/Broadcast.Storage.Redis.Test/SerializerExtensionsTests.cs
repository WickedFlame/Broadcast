using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using NUnit.Framework;
using Polaroider;
using StackExchange.Redis;

namespace Broadcast.Storage.Redis.Test
{
	public class SerializerExtensionsTests
	{
		[Test]
		public void SerializerExtensions_Deserialize_To_DataObject()
		{
			var hash = new[]
			{
				new HashEntry("one", "1"),
				new HashEntry("two", "2")
			};

			var deserialized = hash.DeserializeRedis<DataObject>();

			Assert.AreEqual(1, deserialized["one"]);
			Assert.AreEqual(2, deserialized["two"]);
		}

		[Test]
		public void SerializerExtensions_Deserialize_To_String()
		{
			var hash = new[]
			{
				new HashEntry("one", "1")
			};

			var deserialized = hash.DeserializeRedis<string>();

			Assert.AreEqual("1", deserialized);
		}

		[Test]
		public void SerializerExtensions_Deserialize_To_Object()
		{
			var hash = new[]
			{
				new HashEntry("Id", "1"),
				new HashEntry("Value", "2")
			};

			var deserialized = hash.DeserializeRedis<StorageModel>();

			Assert.AreEqual(1, deserialized.Id);
			Assert.AreEqual("2", deserialized.Value);
		}

		[Test]
		[UpdateSnapshot]
		public void SerializerExtensions_Serialize_Task()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("test"));
			task.Time = TimeSpan.FromSeconds(20);

			var serialized = task.SerializeToRedis();

			serialized.MatchSnapshot();
		}

		[Test]
		[UpdateSnapshot]
		public void SerializerExtensions_Deserialize_Task()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("test"));
			task.Time = TimeSpan.FromSeconds(20);
			var serialized = task.SerializeToRedis();

			var deserialized = serialized.DeserializeRedis<BroadcastTask>();


			var options = SnapshotOptions.Create(o => o.AddFormatter<MethodInfo>(m => $"{m.Name}({string.Join(',', m.GetParameters().Select(x => x.ParameterType.FullName))})"));

			deserialized.MatchSnapshot(options);
		}

		public class StorageModel
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}
