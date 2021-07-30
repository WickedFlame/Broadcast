using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using StackExchange.Redis;

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
			var properties = obj.GetType().GetProperties();

			if (!properties.Any() || obj is string)
			{
				return new[] { new HashEntry(obj.GetType().Name, obj.ToString()) };
			}

			var hashset = properties
				.Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
				.Select(property => new HashEntry(property.Name, property.GetValue(obj)
					.ToString())).ToArray();

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
				return default(T);
			}

			var properties = typeof(T).GetProperties();

			if (!properties.Any() || typeof(T) == typeof(string))
			{
				var entry = hashEntries.FirstOrDefault();
				if (entry.Equals(new HashEntry()))
				{
					return default(T);
				}

				return (T)TypeConverter.Convert(typeof(T), entry.Value.ToString());
			}


			var obj = Activator.CreateInstance(typeof(T));
			//TODO: Refactor this
			if (obj is DataObject dobj)
			{
				foreach (var hash in hashEntries)
				{
					//TODO: try convert to int if possible?
					object value = hash.Value;
					if (int.TryParse(hash.Value, out var i))
					{
						value = i;
					}

					dobj.Add(hash.Name, value);
				}
			}
			else
			{
				foreach (var property in properties)
				{
					var entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
					if (entry.Equals(new HashEntry()))
					{
						continue;
					}

					property.SetValue(obj, TypeConverter.Convert(property.PropertyType, entry.Value.ToString()));
				}
			}

			return (T)obj;
		}
	}
}
