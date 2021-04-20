using System;
using System.Collections.Generic;
using System.Text;

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
			: this(key, null)
		{
		}

		/// <summary>
		/// Creates a new instance of the StorageKey
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serverName"></param>
		public StorageKey(string key, string serverName)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(key);
			}

			Key = key;
			ServerName = serverName;
		}

		/// <summary>
		/// Gets or sets the ServerName
		/// </summary>
		public string ServerName { get; set; }

		/// <summary>
		/// Gets or sets the Key for the item in the storage
		/// </summary>
		public string Key { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(ServerName))
			{
				return Key.ToLower();
			}

			return $"{ServerName}:{Key}".ToLower();
		}
	}
}
