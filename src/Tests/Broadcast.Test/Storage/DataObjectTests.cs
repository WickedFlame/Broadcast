using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Storage;
using NUnit.Framework;

namespace Broadcast.Test.Storage
{
	public class DataObjectTests
	{
		[Test]
		public void DataObject_ctor()
		{
			Assert.DoesNotThrow(() => new DataObject());
		}

		[Test]
		public void DataObject_Add()
		{
			var obj = new DataObject();

			obj.Add("key", 1);

			Assert.IsTrue(obj.Single().Key == "key" && (int) obj.Single().Value == 1);
		}

		[Test]
		public void DataObject_Add_CollectionInitializer()
		{
			var obj = new DataObject
			{
				{"key", 1}
			};

			Assert.IsTrue(obj.Single().Key == "key" && (int)obj.Single().Value == 1);
		}

		[Test]
		public void DataObject_Add_Enumerable()
		{
			var obj = new DataObject
			{
				{"key", 1}
			};

			Assert.IsNotEmpty(obj);
		}

		[Test]
		public void DataObject_Indexer_Set()
		{
			var obj = new DataObject();
			obj["key"] = 1;

			Assert.IsTrue(obj.Single().Key == "key" && (int)obj.Single().Value == 1);
		}

		[Test]
		public void DataObject_Indexer_Get()
		{
			var obj = new DataObject
			{
				{"key", 1}
			};

			Assert.AreEqual(1, obj["key"]);
		}

		[Test]
		public void DataObject_Indexer_Overwrite()
		{
			var obj = new DataObject
			{
				{"key", 2}
			};
			obj["key"] = 1;

			Assert.AreEqual(1, obj["key"]);
		}
	}
}
