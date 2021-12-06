using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Storage.Inmemory;
using NUnit.Framework;

namespace Broadcast.Test.Storage
{
	public class ListItemTests
	{
		[Test]
		public void ListItem_Ctor()
		{
			Assert.IsNotNull(new ListItem());
		}

		[Test]
		public void ListItem_SetValue()
		{
			var item = new ListItem();
			item.SetValue("value");

            Assert.That(item.Count, Is.EqualTo(1));
		}

		[Test]
		public void ListItem_Multiple_Values()
		{
			var item = new ListItem();
			item.SetValue("one");
			item.SetValue("two");

			Assert.IsTrue(((List<object>) item.GetValue()).Count() == 2);
		}

		[Test]
		public void ListItem_GetValue()
		{
			var item = new ListItem();
			item.SetValue("value");

			Assert.That(item.GetValue(), Is.EquivalentTo(new []{"value"}));
		}

		[Test]
		public void ListItem_GetValue_IEnumerable()
		{
			var item = new ListItem();
			item.SetValue("value");

			var tmp = item.GetValue().GetType();

			Assert.IsAssignableFrom<List<object>>(item.GetValue());

		}

		[Test]
		public void ListItem_GetValue_EnsureSame()
		{
			var items = new[]
			{
				"one",
				"two"
			};
			var item = new ListItem();
			item.SetValue(items[0]);
			item.SetValue(items[1]);

			var resolved = item.GetValue() as List<object>;
			for (var i = 0; i < resolved.Count; i++)
			{
				Assert.AreSame(items[i], resolved[i]);
			}
		}
	}
}
