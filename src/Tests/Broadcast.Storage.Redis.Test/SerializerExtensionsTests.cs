using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
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

		public class StorageModel
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}
