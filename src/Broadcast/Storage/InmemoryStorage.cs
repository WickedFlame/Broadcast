using Broadcast.Storage.Inmemory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Storage
{
	/// <summary>
	/// The default Storage
	/// </summary>
	public class InmemoryStorage : IStorage
	{
		private readonly Dictionary<string, IStorageItem> _store = new Dictionary<string, IStorageItem>();

		/// <inheritdoc/>
		public void AddToList<T>(StorageKey key, T value)
		{
			var internalKey = key.ToString();
			if (!_store.ContainsKey(internalKey))
			{
				_store.Add(internalKey, new ListItem());
			}

			var lst = _store[internalKey] as ListItem;
			if (lst == null)
			{
				throw new InvalidOperationException($"{internalKey} is not a list");
			}

			lst.SetValue(value);
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetList<T>(StorageKey key)
		{
			if (_store.ContainsKey(key.ToString()))
			{
				if (_store[key.ToString()].GetValue() is List<object> items)
				{
					return items.Cast<T>();
				}
			}

			return (IEnumerable<T>)default;
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(StorageKey key, int count)
		{
			if (_store.ContainsKey(key.ToString()))
			{
				if (_store[key.ToString()].GetValue() is List<object> items)
				{
					items.RemoveRange(0, count);
				}
			}
		}

		/// <inheritdoc/>
		public void Set<T>(StorageKey key, T value)
		{
			_store[key.ToString()] = new ValueItem(value);
		}

		/// <inheritdoc/>
		public T Get<T>(StorageKey key)
		{
			if (_store.ContainsKey(key.ToString()))
			{
				return (T)_store[key.ToString()].GetValue();
			}

			return (T)default;
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetKeys(StorageKey key)
		{
			return _store.Keys.Where(k => k.StartsWith(key.ToString()));
		}
	}
}
