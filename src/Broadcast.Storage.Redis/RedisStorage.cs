using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace Broadcast.Storage.Redis
{
	public class RedisStorage : IStorage
	{
		private readonly RedisStorageOptions _options;
		
		private readonly IDatabase _database;
		private readonly IServer _server;

		private readonly RedisSubscription _subscription;

		public RedisStorage(IConnectionMultiplexer connectionMultiplexer, RedisStorageOptions options = null)
		{
			if (connectionMultiplexer == null)
			{
				throw new ArgumentNullException(nameof(connectionMultiplexer));
			}

			_options = options ?? new RedisStorageOptions();
			_database = connectionMultiplexer.GetDatabase(_options.Db);
			_server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints(true).FirstOrDefault());

			_subscription = new RedisSubscription(connectionMultiplexer.GetSubscriber());
		}

		public RedisSubscription Subscription => _subscription;

		/// <inheritdoc/>
		public void AddToList(StorageKey key, string value)
		{
			_database.ListRightPushAsync(CreateKey(key), value);
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetList(StorageKey key)
		{
			var list = _database.ListRange(CreateKey(key));
			return list.Select(l => l.ToString());
		}

		/// <inheritdoc/>
		public bool RemoveFromList(StorageKey key, string item)
		{
			_database.ListRemoveAsync(CreateKey(key), item);

			return true;
		}

		/// <inheritdoc/>
		public bool TryFetchNext(StorageKey source, StorageKey destination, out string item)
		{
			item = _database.ListRightPopLeftPush(CreateKey(source), CreateKey(destination));
			if (string.IsNullOrEmpty(item))
			{
				return false;
			}

			return true;
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(StorageKey key, int count)
		{
			for (var i = 0; i < count; i++)
			{
				_database.ListLeftPopAsync(CreateKey(key));
			}
		}

		/// <inheritdoc/>
		public void Set<T>(StorageKey key, T value)
		{
			_database.HashSetAsync(CreateKey(key), value.SerializeToRedis());
		}

		/// <inheritdoc/>
		public T Get<T>(StorageKey key)
		{
			var hash = _database.HashGetAll(CreateKey(key));

			return hash.DeserializeRedis<T>();
		}

		/// <inheritdoc/>
		public void SetValues(StorageKey key, DataObject values)
		{
			var list = values.Where(v => v.Value != null)
				.Select(v => new HashEntry(v.Key, v.Value.ToString()))
				.ToArray();

			_database.HashSetAsync(CreateKey(key), list);
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetKeys(StorageKey key)
		{
			return _server.Keys(_options.Db, $"{CreateKey(key)}*").Select(k => k.ToString());
		}

		/// <inheritdoc/>
		public void Delete(StorageKey key)
		{
			_database.KeyDeleteAsync(CreateKey(key));
		}

		/// <inheritdoc/>
		public void RegisterSubscription(ISubscription subscription)
		{
			_subscription.RegisterSubscription(subscription);
		}

		/// <inheritdoc/>
		public void PropagateEvent(StorageKey key)
		{
			_database.PublishAsync(RedisSubscription.Channel, CreateKey(key));
		}

		private string CreateKey(StorageKey key) => key.ToString().StartsWith(_options.KeySpacePrefix) ? key.ToString() : $"{_options.KeySpacePrefix}:{key}";
	}
}
