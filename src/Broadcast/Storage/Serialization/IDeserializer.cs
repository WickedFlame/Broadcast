using System.Collections.Generic;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A deserializer for deserializing a list of <see cref="HashValue"/> to an object
	/// </summary>
	public interface IDeserializer
	{
		/// <summary>
		/// Deserialize a list of <see cref="HashValue"/> to an object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="values"></param>
		/// <returns></returns>
		object Deserialize<T>(IEnumerable<HashValue> values);
	}
}
