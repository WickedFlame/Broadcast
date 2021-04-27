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
		private readonly object _lockHandle = new object();

		private readonly Dictionary<string, IStorageItem> _store = new Dictionary<string, IStorageItem>();

		/// <inheritdoc/>
		public void AddToList<T>(StorageKey key, T value)
		{
			lock (_lockHandle)
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
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetList<T>(StorageKey key)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(key.ToString()))
				{
					if (_store[key.ToString()].GetValue() is List<object> items)
					{
						return items.Cast<T>();
					}
				}

				return Enumerable.Empty<T>();
			}
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(StorageKey key, int count)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(key.ToString()))
				{
					if (_store[key.ToString()] is ListItem list)
					{
						if (list.Items.Count() < count)
						{
							count = list.Items.Count();
						}

						list.Items.RemoveRange(0, count);
					}
				}
			}
		}

		/// <inheritdoc/>
		public void Set<T>(StorageKey key, T value)
		{
			lock (_lockHandle)
			{
				_store[key.ToString()] = new ValueItem(value);
			}
		}

		/// <inheritdoc/>
		public T Get<T>(StorageKey key)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(key.ToString()))
				{
					var item = _store[key.ToString()].GetValue();
					if (item != null && item.GetType() == typeof(T))
					{
						return (T) item;
					}
				}

				return (T) default;
			}
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetKeys(StorageKey key)
		{
			lock (_lockHandle)
			{
				return _store.Keys.Where(k => k.StartsWith(key.ToString()));
			}
		}

		/// <summary>
		/// Delete the storage entry
		/// </summary>
		/// <param name="key"></param>
		public void Delete(StorageKey key)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(key.ToString()))
				{
					_store.Remove(key.ToString());
				}
			}
		}
	}
}
