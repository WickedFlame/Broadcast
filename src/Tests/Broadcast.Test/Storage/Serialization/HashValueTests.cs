using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Storage.Serialization;
using NUnit.Framework;

namespace Broadcast.Test.Storage.Serialization
{
	public class HashValueTests
	{
		[Test]
		public void HashValue_ctor()
		{
			Assert.DoesNotThrow(() => new HashValue("name", "value"));
		}

		[Test]
		public void HashValue_Name()
		{
			var value = new HashValue("name", "value");

			Assert.AreEqual("name", value.Name);
		}

		[Test]
		public void HashValue_Value()
		{
			var value = new HashValue("name", "value");

			Assert.AreEqual("value", value.Value);
		}
	}
}
