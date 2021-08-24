using System.Collections.Generic;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A deserializer for serializing and deserializing a <see cref="HashValue"/> to a <see cref="DataObject"/>
	/// </summary>
	public class DataObjectSerializer : IDeserializer
	{
		/// <summary>
		/// Deserialize a list of <see cref="HashValue"/> to a <see cref="DataObject"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="hashEntries"></param>
		/// <returns></returns>
		public object Deserialize<T>(IEnumerable<HashValue> hashEntries)
		{
			if (typeof(T) != typeof(DataObject))
			{
				return null;
			}

			var obj = new DataObject();
			foreach (var hash in hashEntries)
			{
				object value = hash.Value;
				if (int.TryParse(hash.Value, out var i))
				{
					value = i;
				}

				obj.Add(hash.Name, value);
			}

			return obj;
		}
	}
}
