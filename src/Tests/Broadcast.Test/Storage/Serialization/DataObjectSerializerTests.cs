using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using Broadcast.Storage.Serialization;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Test.Storage.Serialization
{
	public class DataObjectSerializerTests
	{
		[Test]
		public void DataObjectSerializer_ctor()
		{
			Assert.DoesNotThrow(() => new DataObjectSerializer());
		}

		[Test]
		public void DataObjectSerializer_Deserialize()
		{
			var hash = new List<HashValue>
			{
				new HashValue("one", "one"),
				new HashValue("two", "two")
			};

			var serializer = new DataObjectSerializer();
			var data = serializer.Deserialize<DataObject>(hash);

			data.MatchSnapshot();
		}

		[Test]
		public void DataObjectSerializer_Deserialize_Int()
		{
			var hash = new List<HashValue>
			{
				new HashValue("one", "1"),
				new HashValue("two", "2")
			};

			var serializer = new DataObjectSerializer();
			var data = serializer.Deserialize<DataObject>(hash) as DataObject;

			Assert.AreEqual(1, data["one"]);
			Assert.AreEqual(2, data["two"]);
		}

		[Test]
		public void DataObjectSerializer_Deserialize_WrongType()
		{
			var hash = new List<HashValue>
			{
				new HashValue("one", "one"),
				new HashValue("two", "two")
			};

			var serializer = new DataObjectSerializer();
			Assert.IsNull(serializer.Deserialize<BroadcastTask>(hash));
		}

		[Test]
		public void DataObjectSerializer_Serialize()
		{
			var data = new DataObject
			{
				{"one", 1}, 
				{"two", "two"}
			};

			var serializer = new DataObjectSerializer();
			var hash = serializer.Serialize(data);

			hash.MatchSnapshot();
		}

		[Test]
		public void DataObjectSerializer_Serialize_WrongType()
		{
			var serializer = new DataObjectSerializer();
			var hash = serializer.Serialize("test");

			Assert.IsNull(hash);
		}
	}
}
