using System;
using Broadcast.EventSourcing;
using Broadcast.Setup;
using Broadcast.Storage.Redis;
using StackExchange.Redis;

namespace Broadcast
{
	/// <summary>
	/// Redis Extensions for the <see cref="ServerSetup"/>
	/// </summary>
	public static class RedisServerSetupExtensions
	{
		/// <summary>
		/// Configures a storage that uses Redis to store the tasks and states
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="connectionString"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static ServerSetup UseRedisStorage(this ServerSetup setup, string connectionString, RedisStorageOptions options = null)
		{
			if (setup == null)
			{
				throw new ArgumentNullException(nameof(setup));
			}

			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			var redisOptions = ConfigurationOptions.Parse(connectionString);
			options = options ?? new RedisStorageOptions
			{
				Db = redisOptions.DefaultDatabase ?? 0
			};

			var connectionMultiplexer = ConnectionFactory.Connect(connectionString);
			return UseRedisStorage(setup, connectionMultiplexer, options);
		}

		/// <summary>
		/// Configures a storage that uses Redis to store the tasks and states
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="connectionMultiplexer"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static ServerSetup UseRedisStorage(this ServerSetup setup, IConnectionMultiplexer connectionMultiplexer, RedisStorageOptions options = null)
		{
			if (setup == null)
			{
				throw new ArgumentNullException(nameof(setup));
			}

			if (connectionMultiplexer == null)
			{
				throw new ArgumentNullException(nameof(connectionMultiplexer));
			}

			var storage = new RedisStorage(connectionMultiplexer, options);
			var store = new TaskStore(storage);
			
			setup.Register<ITaskStore>(store);

			return setup;
		}
	}
}
