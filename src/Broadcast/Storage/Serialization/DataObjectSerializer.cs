using System;
using System.Collections.Generic;
using System.Linq;
using Broadcast.EventSourcing;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A serializer or deserializer for serializing and deserializing a <see cref="HashValue"/> to a <see cref="DataObject"/>
	/// </summary>
	public class DataObjectSerializer : ISerializer, IDeserializer
	{
		/// <summary>
		/// Serialize a <see cref="BroadcastTask"/> to a list of <see cref="HashValue"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public IEnumerable<HashValue> Serialize(object obj)
		{
			if (obj is DataObject data)
			{
				return data.Where(p => p.Value != null)
					.Select(p => new HashValue(p.Key, p.Value.ToString()));
			}

			return null;
		}

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
				else if (DateTime.TryParse(hash.Value, out var dte))
				{
					value = dte;
				}
				else if (Enum.TryParse<TaskState>(hash.Value, out var state))
				{
					value = state;
				}

				obj.Add(hash.Name, value);
			}

			return obj;
		}
	}
}
