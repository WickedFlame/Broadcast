using Broadcast.Storage.Inmemory;
using System;
using System.Collections.Generic;
using System.Linq;
using Broadcast.Storage.Serialization;

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
		public void AddToList(StorageKey key, string value)
		{
			lock (_lockHandle)
			{
				AddToList(key, new ValueItem(value));
			}
		}

		private void AddToList(StorageKey key, ValueItem value)
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

			lst.Set(value);
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetList(StorageKey key)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(key.ToString()))
				{
					if (_store[key.ToString()].GetValue() is List<object> items)
					{
						return items.Cast<string>();
					}
				}

				return Enumerable.Empty<string>();
			}
		}

		/// <inheritdoc/>
		public bool RemoveFromList(StorageKey key, string item)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(key.ToString()))
				{
					if (!(_store[key.ToString()] is ListItem list))
					{
						return false;
					}

					var stored = list.Items.FirstOrDefault(i => ((string)i.GetValue()).Equals(item));
					if (stored == null)
					{
						return false;
					}

					list.Items.Remove(stored);
					return true;
				}

				return false;
			}
		}

		/// <inheritdoc/>
		public bool TryFetchNext(StorageKey source, StorageKey destination, out string item)
		{
			lock (_lockHandle)
			{
				if (_store.ContainsKey(source.ToString()))
				{
					var stored = _store[source.ToString()];
					if (stored is ListItem list)
					{
						if (list.Items.FirstOrDefault() is ValueItem valueItem)
						{
							list.Items.Remove(valueItem);
							AddToList(destination, valueItem);

							item = valueItem.Deserialize<string>();
							return true;
						}
					}
					else if (stored is ValueItem valueItem)
					{
						_store.Remove(source.ToString());
						_store[destination.ToString()] = valueItem;

						item = valueItem.Deserialize<string>();
						return true;
					}
				}

				item = null;
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
				// serialize object to ensure a breake of the references
				// this simulates the same behaviour we have when using an external storage
				_store[key.ToString()] = new ValueItem(value.Serialize());

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
					return _store[key.ToString()].Deserialize<T>();
				}

				return default;
			}
		}

		/// <inheritdoc/>
		public void SetValues(StorageKey key, DataObject values)
		{
			// serialization is done in Get<> or Set

			// get original object from storage
			var stored = Get<DataObject>(key) ?? new DataObject();

			// merge objects
			foreach (var item in values)
			{
				stored[item.Key] = item.Value;
			}

			// save objects
			Set(key, stored);
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetKeys(StorageKey key)
		{
			lock (_lockHandle)
			{
				return _store.Keys.Where(k => k.StartsWith(key.ToString())).ToList();
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
