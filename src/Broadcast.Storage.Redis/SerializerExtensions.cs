using System.Linq;
using StackExchange.Redis;
using Broadcast.Storage.Serialization;

namespace Broadcast.Storage.Redis
{
	/// <summary>
	/// Extensions for object
	/// </summary>
	public static class SerializerExtensions
	{
		/// <summary>
		/// Serialize objects to HashEntries
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static HashEntry[] SerializeToRedis(this object obj)
		{
			var hashset = obj.Serialize()
				.Select(h => new HashEntry(h.Name, h.Value))
				.ToArray();

			return hashset;
		}

		/// <summary>
		/// Deserialize Redis hashEntries to Objects
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="hashEntries"></param>
		/// <returns></returns>
		public static T DeserializeRedis<T>(this HashEntry[] hashEntries)
		{
			if (!hashEntries.Any())
			{
				return default;
			}

			var item = hashEntries.Select(h => new HashValue(h.Name, h.Value)).Deserialize<T>();
			return item;
		}
	}
}
