using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A deserializer for deserializing a <see cref="HashValue"/> to a string
	/// </summary>
	public class StringSerializer : IDeserializer
	{
		/// <summary>
		/// Deserialize a list of <see cref="HashValue"/> to a string. If the list contains multiple elements the first is taken
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="hashEntries"></param>
		/// <returns></returns>
		public object Deserialize<T>(IEnumerable<HashValue> hashEntries)
		{
			if (typeof(T) != typeof(string))
			{
				return null;
			}

			var entry = hashEntries.FirstOrDefault();
			if (entry == null)
			{
				return string.Empty;
			}

			return (T)TypeConverter.Convert(typeof(T), entry.Value.ToString());
		}
	}
}
