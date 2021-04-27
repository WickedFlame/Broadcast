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

		private readonly Dictionary<string, IStorageItem> _store;
		private readonly List<ISubscription> _subscriptions;

		public InmemoryStorage()
		{
			_store = new Dictionary<string, IStorageItem>();
			_subscriptions = new List<ISubscription>();
		}

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
		public bool RemoveFromList<T>(StorageKey key, T item)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(key.ToString()))
				{
					if (_store[key.ToString()] is ListItem list)
					{
						foreach (var stored in list.Items)
						{
							if (((T)stored.GetValue()).Equals(item))
							{
								list.Items.Remove(stored);
								return true;
							}
						}
					}
				}

				return false;
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

				var stringKey = key.ToString().ToLower();
				foreach (var dispatcher in _subscriptions.Where(d => stringKey.Contains(d.EventKey.ToLower())))
				{
					dispatcher.RaiseEvent();
				}
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
					if (item != null && item is T item1)
					{
						return item1;
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

		/// <summary>
		/// Register a <see cref="ISubscription"/> to the storage.
		/// The subscription gets called as soon as a item is added with the key in the event
		/// </summary>
		/// <param name="subscription"></param>
		public void RegisterSubscription(ISubscription subscription)
		{
			_subscriptions.Add(subscription);
		}
	}
}
