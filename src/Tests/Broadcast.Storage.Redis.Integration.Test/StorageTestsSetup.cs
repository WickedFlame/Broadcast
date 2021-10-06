using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Storage.Redis;
using NUnit.Framework;
using StackExchange.Redis;

namespace Broadcast.Storage.Integration.Test
{
	public partial class StorageTests
	{
		private IConnectionMultiplexer _connectionMultiplexer;

		[OneTimeSetUp]
		public void Setup()
		{
			try
			{
				_connectionMultiplexer = ConnectionFactory.Connect("localhost:6379");
			}
			catch (Exception)
			{
				// do nothing
			}
		}

		[OneTimeTearDown]
		public void TearDowns()
		{
			if (_connectionMultiplexer == null)
			{
				return;
			}

			var db = _connectionMultiplexer.GetDatabase(0);
			db.KeyDelete("{broadcast}:list_objects:storage");
			db.KeyDelete("{broadcast}:list_distinct:storage");
			db.KeyDelete("{broadcast}:fetch_key1");
			db.KeyDelete("{broadcast}:fetch_key2");
		}

		public IStorage BuildStorage()
		{
			if (_connectionMultiplexer == null)
			{
				Assert.Ignore("Redis is not availiable");
				return null;
			}

			try
			{
				return new RedisStorage(_connectionMultiplexer, new RedisStorageOptions{Db = 0});
			}
			catch
			{
				Assert.Ignore();
				return null;
			}
		}
	}
}
