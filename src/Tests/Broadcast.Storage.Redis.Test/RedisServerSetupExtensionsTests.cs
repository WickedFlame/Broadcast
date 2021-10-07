using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Setup;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace Broadcast.Storage.Redis.Test
{
	public class RedisServerSetupExtensionsTests
	{
		private Mock<IConnectionMultiplexer> _multiplexer;

		[SetUp]
		public void Setup()
		{
			_multiplexer = new Mock<IConnectionMultiplexer>();
			_multiplexer.Setup(exp => exp.GetSubscriber(null)).Returns(new Mock<ISubscriber>().Object);

			ConnectionFactory.Connect = s => _multiplexer.Object;
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer()
		{
			var setup = new ServerSetup();
			Assert.DoesNotThrow(() => setup.UseRedisStorage(_multiplexer.Object, new RedisStorageOptions()));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer_StoredInContext()
		{
			var setup = new ServerSetup();
			setup.UseRedisStorage(_multiplexer.Object, new RedisStorageOptions());

			Assert.NotNull(setup.Context[nameof(ITaskStore)]);
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer_TaskStore_Instance()
		{
			var setup = new ServerSetup();
			setup.UseRedisStorage(_multiplexer.Object, new RedisStorageOptions());

			Assert.IsAssignableFrom<TaskStore>(setup.Context[nameof(ITaskStore)]);
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer_NoOptions()
		{
			var setup = new ServerSetup();
			Assert.DoesNotThrow(() => setup.UseRedisStorage(_multiplexer.Object));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer_NullOptions()
		{
			var setup = new ServerSetup();
			Assert.DoesNotThrow(() => setup.UseRedisStorage(_multiplexer.Object, null));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer_NullMultiplexer()
		{
			var setup = new ServerSetup();
			Assert.Throws<ArgumentNullException>(() => setup.UseRedisStorage((IConnectionMultiplexer) null, null));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer_NullSetup()
		{
			Assert.Throws<ArgumentNullException>(() => ((ServerSetup)null).UseRedisStorage(_multiplexer.Object));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_Multiplexer_ContainsKey()
		{
			var setup = new ServerSetup();
			setup.UseRedisStorage(_multiplexer.Object, new RedisStorageOptions());

			Assert.IsTrue(setup.Context.ContainsKey(nameof(ITaskStore)));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString()
		{
			var setup = new ServerSetup();
			Assert.DoesNotThrow(() => setup.UseRedisStorage("localhost:6379", new RedisStorageOptions()));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_StoredInContext()
		{
			var setup = new ServerSetup();
			setup.UseRedisStorage("localhost:6379", new RedisStorageOptions());

			Assert.NotNull(setup.Context[nameof(ITaskStore)]);
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_RedisStorage_Type()
		{
			var setup = new ServerSetup();
			setup.UseRedisStorage("localhost:6379", new RedisStorageOptions());

			Assert.IsAssignableFrom<TaskStore>(setup.Context[nameof(ITaskStore)]);
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_NoOptions()
		{
			var setup = new ServerSetup();
			Assert.DoesNotThrow(() => setup.UseRedisStorage("localhost:6379"));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_NullOptions()
		{
			var setup = new ServerSetup();
			Assert.DoesNotThrow(() => setup.UseRedisStorage("localhost:6379", null));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_NullConnectionString()
		{
			var setup = new ServerSetup();
			Assert.Throws<ArgumentNullException>(() => setup.UseRedisStorage((string)null, null));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_EmptyConnectionString()
		{
			var setup = new ServerSetup();
			Assert.Throws<ArgumentNullException>(() => setup.UseRedisStorage(string.Empty, null));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_NullSetup()
		{
			Assert.Throws<ArgumentNullException>(() => ((ServerSetup)null).UseRedisStorage("localhost:6379"));
		}

		[Test]
		public void ServerSetup_UseRedisStorage_ConnectionString_ContainsKey()
		{
			var setup = new ServerSetup();
			setup.UseRedisStorage("localhost:6379", new RedisStorageOptions());

			Assert.IsTrue(setup.Context.ContainsKey(nameof(ITaskStore)));
		}
	}
}
