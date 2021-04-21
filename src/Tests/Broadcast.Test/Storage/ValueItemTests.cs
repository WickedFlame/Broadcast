using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Storage.Inmemory;
using NUnit.Framework;

namespace Broadcast.Test.Storage
{
	public class ValueItemTests
	{
		[Test]
		public void ValueItem_Ctor()
		{
			Assert.IsNotNull(new ValueItem());
		}

		[Test]
		public void ValueItem_SetValue()
		{
			var item = new ValueItem();
			item.SetValue("value");
			Assert.AreEqual(item.Value, "value");
		}

		[Test]
		public void ValueItem_GetValue()
		{
			var item = new ValueItem();
			item.SetValue("value");
			Assert.AreEqual(item.GetValue(), "value");
		}
	}
}
