using System;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A serializer or deserializer for serializing and deserializing a <see cref="HashValue"/> to a any object
	/// </summary>
	public class ObjectSerializer : ISerializer, IDeserializer
	{
		/// <summary>
		/// Serialize a object to a list of <see cref="HashValue"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public IEnumerable<HashValue> Serialize(object obj)
		{
			var properties = obj.GetType().GetProperties();

			if (!properties.Any() || obj is string)
			{
				return new[] { new HashValue(obj.GetType().Name, obj.ToString()) };
			}

			var hashset = properties
				.Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
				.Select(property => new HashValue(property.Name, property.GetValue(obj)
					.ToString())).ToArray();

			return hashset;
		}

		/// <summary>
		/// Deserialize a list of <see cref="HashValue"/> to a object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="hashEntries"></param>
		/// <returns></returns>
		public object Deserialize<T>(IEnumerable<HashValue> hashEntries)
		{
			var obj = Activator.CreateInstance(typeof(T));
			var properties = typeof(T).GetProperties();
			foreach (var property in properties)
			{
				var entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
				if (entry == null)
				{
					continue;
				}

				property.SetValue(obj, TypeConverter.Convert(property.PropertyType, entry.Value.ToString()));
			}

			return obj;
		}
	}
}
