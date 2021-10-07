using System;

namespace Broadcast.Storage
{
	/// <summary>
	/// The key used for the storage
	/// </summary>
	public class StorageKey
	{
		/// <summary>
		/// Creates a new instance of the StorageKey
		/// </summary>
		/// <param name="key"></param>
		public StorageKey(string key)
		{
			Key = key ?? throw new ArgumentNullException(nameof(key));
		}

		/// <summary>
		/// Creates a new instance of the StorageKey
		/// </summary>
		/// <param name="key"></param>
		/// <param name="prefix"></param>
		public StorageKey(string key, string prefix)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(key);
			}

			if (string.IsNullOrEmpty(prefix))
			{
				throw new ArgumentNullException(prefix);
			}

			Key = key;
			Prefix = prefix;
		}

		/// <summary>
		/// Gets or sets the ServerName
		/// </summary>
		public string Prefix { get; set; }

		/// <summary>
		/// Gets or sets the Key for the item in the storage
		/// </summary>
		public string Key { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Prefix))
			{
				return Key.ToLower();
			}

			return $"{Prefix}:{Key}".ToLower();
		}
	}
}
