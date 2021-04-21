using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Storage;
using NUnit.Framework;

namespace Broadcast.Test.Storage
{
	public class StorageKeyTests
	{
		[Test]
		public void StorageKey_Valid()
		{
			Assert.DoesNotThrow(() => new StorageKey("pipelineId", "key"));
		}

		[Test]
		public void StorageKey_Servername_Valid()
		{
			Assert.DoesNotThrow(() => new StorageKey("key", "serverName"));
		}

		[Test]
		public void StorageKey_NUll_Server()
		{
			Assert.Throws<ArgumentNullException>(() => new StorageKey("key", null));
		}

		[Test]
		public void StorageKey_NUll_Key()
		{
			Assert.Throws<ArgumentNullException>(() => new StorageKey(null, "server"));
		}

		[Test]
		public void StorageKey_ToString()
		{
			Assert.AreEqual("server:key", new StorageKey("key", "server").ToString());
		}

		[Test]
		public void StorageKey_ToString_Case()
		{
			Assert.AreEqual("server:key", new StorageKey("KEY", "Server").ToString());
		}
	}
}
