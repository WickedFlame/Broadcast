using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Storage.Serialization;

namespace Broadcast.Storage.Inmemory
{
	internal static class StorageItemExtensions
	{
		public static T Deserialize<T>(this IStorageItem storageItem)
		{
			if (storageItem.GetValue() is IEnumerable<HashValue> hash)
			{
				var item = hash.Deserialize<T>();
				return item;
			}

			return storageItem.GetValue() is T value ? value : default;
		}
	}
}
