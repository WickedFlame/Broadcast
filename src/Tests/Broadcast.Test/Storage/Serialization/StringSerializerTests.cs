using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Storage.Serialization;
using NUnit.Framework;

namespace Broadcast.Test.Storage.Serialization
{
	public class StringSerializerTests
	{
		[Test]
		public void StringSerializer_ctor()
		{
			Assert.DoesNotThrow(() => new StringSerializer());
		}

		[Test]
		public void StringSerializer_Deserialize()
		{
			var serializer = new StringSerializer();

			var value = serializer.Deserialize<string>(new[] {new HashValue("property", "value")});
			Assert.AreEqual("value", value);
		}

		[Test]
		public void StringSerializer_Deserialize_MultipleValues()
		{
			var serializer = new StringSerializer();

			// deserializing a string takes the first property value
			var value = serializer.Deserialize<string>(new[] { new HashValue("property", "value"), new HashValue("property2", "invalid") });
			Assert.AreEqual("value", value);
		}

		[Test]
		public void StringSerializer_Deserialize_InvalidType()
		{
			var serializer = new StringSerializer();

			var value = serializer.Deserialize<StringSerializerTests>(new[] { new HashValue("property", "value") });
			Assert.IsNull(value);
		}

		[Test]
		public void StringSerializer_Deserialize_CheckType()
		{
			var serializer = new StringSerializer();

			var value = serializer.Deserialize<StringSerializerTests>(new[] { new HashValue("property", "value") });
			Assert.IsInstanceOf<string>(value);
		}

		[Test]
		public void StringSerializer_Deserialize_NoValues()
		{
			var serializer = new StringSerializer();

			var value = serializer.Deserialize<string>(new List<HashValue>()) as string;
			Assert.IsEmpty(value);
		}
	}
}
