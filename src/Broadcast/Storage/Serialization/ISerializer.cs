using System.Collections.Generic;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A serializer for serializing objcts to a list of <see cref="HashValue"/>. Used to serialize data that is stored
	/// </summary>
	public interface ISerializer
	{
		/// <summary>
		/// Serialize an object to a list of <see cref="HashValue"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		IEnumerable<HashValue> Serialize(object obj);
	}
}
