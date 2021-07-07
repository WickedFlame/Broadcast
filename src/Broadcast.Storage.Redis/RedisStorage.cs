using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Broadcast.EventSourcing;
using Broadcast.Server;
using StackExchange.Redis;

namespace Broadcast.Storage.Redis
{
	public class RedisStorage : IStorage
	{
		private readonly RedisStorageOptions _options;
		private readonly IDatabase _database;
		private readonly IServer _server;

		public RedisStorage(IConnectionMultiplexer connectionMultiplexer, RedisStorageOptions options = null)
		{
			if (connectionMultiplexer == null)
			{
				throw new ArgumentNullException(nameof(connectionMultiplexer));
			}

			_options = options ?? new RedisStorageOptions();
			_database = connectionMultiplexer.GetDatabase(_options.Db);
			_server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints(true).FirstOrDefault());
		}

		/// <inheritdoc/>
		public void AddToList<T>(StorageKey key, T value)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetList<T>(StorageKey key)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public bool RemoveFromList<T>(StorageKey key, T item)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public bool TryFetchNext<T>(StorageKey source, StorageKey destination, out T item)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(StorageKey key, int count)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public void Set<T>(StorageKey key, T value)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public T Get<T>(StorageKey key)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public void SetValues(StorageKey key, DataObject values)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetKeys(StorageKey key)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public void Delete(StorageKey key)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public void RegisterSubscription(ISubscription subscription)
		{
			throw new NotImplementedException();
		}
	}
}
