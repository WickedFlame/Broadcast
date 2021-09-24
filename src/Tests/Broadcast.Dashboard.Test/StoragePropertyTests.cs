using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Dashboard.Dispatchers.Models;
using NUnit.Framework;

namespace Broadcast.Dashboard.Test
{
	public class StoragePropertyTests
	{
		[Test]
		public void StorageProperty_ToString()
		{
			var property = new StorageProperty("key", "value");
			Assert.AreEqual("key: value", property.ToString());
		}
	}
}
