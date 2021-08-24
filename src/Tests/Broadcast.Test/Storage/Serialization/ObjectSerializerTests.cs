using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Storage.Serialization;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Test.Storage.Serialization
{
	public class ObjectSerializerTests
	{
		[Test]
		public void ObjectSerializer_ctor()
		{
			Assert.DoesNotThrow(() => new ObjectSerializer());
		}

		[Test]
		public void ObjectSerializer_Serialize()
		{
			var model = new ObjectSerializerTestModel
			{
				Name = "model",
				Id = 1
			};

			var serializer = new ObjectSerializer();
			var hash = serializer.Serialize(model);

			hash.MatchSnapshot();
		}

		[Test]
		public void ObjectSerializer_Serialize_String()
		{
			var serializer = new ObjectSerializer();
			var hash = serializer.Serialize("model");

			Assert.AreEqual(1, hash.Count());
		}

		[Test]
		public void ObjectSerializer_Serialize_String_Key()
		{
			var serializer = new ObjectSerializer();
			var hash = serializer.Serialize("model");

			Assert.AreEqual("String", hash.First().Name);
		}

		[Test]
		public void ObjectSerializer_Serialize_String_Value()
		{
			var serializer = new ObjectSerializer();
			var hash = serializer.Serialize("model");

			Assert.AreEqual("model", hash.First().Value);
		}

		[Test]
		public void ObjectSerializer_Serialize_NullProperty()
		{
			var model = new ObjectSerializerTestModel
			{
				Name = null,
				Id = 1
			};

			var serializer = new ObjectSerializer();
			var hash = serializer.Serialize(model);

			Assert.IsTrue(hash.Single().Name == "Id" && hash.Single().Value == "1");
		}

		[Test]
		public void ObjectSerializer_Deserialize()
		{
			var hash = new List<HashValue>
			{
				new HashValue("Id", "1"),
				new HashValue("Name", "name")
			};

			var serializer = new ObjectSerializer();
			var model = serializer.Deserialize< ObjectSerializerTestModel>(hash);

			model.MatchSnapshot();
		}

		[Test]
		public void ObjectSerializer_Deserialize_MissingValue()
		{
			var hash = new List<HashValue>
			{
				new HashValue("Id", "1")
			};

			var serializer = new ObjectSerializer();
			var model = serializer.Deserialize<ObjectSerializerTestModel>(hash) as ObjectSerializerTestModel;

			Assert.IsNull(model.Name);
		}

		public class ObjectSerializerTestModel
		{
			public string Name{ get; set; }

			public int Id{ get; set; }
		}
	}
}
