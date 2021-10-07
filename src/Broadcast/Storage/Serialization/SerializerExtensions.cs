using System;
using System.Collections.Generic;
using Broadcast.EventSourcing;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// Extensions for serializing and desesrializing objcts to and from <see cref="HashValue"/>
	/// </summary>
	public static class SerializerExtensions
	{
		private static Dictionary<Type, ISerializer> _serializers = new Dictionary<Type, ISerializer>
		{
			{typeof(BroadcastTask), new BroadcastTaskSerializer()},
			{typeof(DataObject), new DataObjectSerializer()}
		};

		private static Dictionary<Type, IDeserializer> _deserializers = new Dictionary<Type, IDeserializer>
		{
			{typeof(BroadcastTask), new BroadcastTaskSerializer()},
			{typeof(DataObject), new DataObjectSerializer()},
			{typeof(string), new StringSerializer()}
		};

		private static ISerializer _defaultSerializer = new ObjectSerializer();
		private static IDeserializer _defaultDeserializer = new ObjectSerializer();

		/// <summary>
		/// Serialize the object to a list of <see cref="HashValue"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static IEnumerable<HashValue> Serialize(this object obj)
		{
			var serializer = _serializers.ContainsKey(obj.GetType()) ? _serializers[obj.GetType()] : _defaultSerializer;
			return serializer.Serialize(obj);
		}

		/// <summary>
		/// Deserialize a list of <see cref="HashValue"/> to a object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="hashEntries"></param>
		/// <returns></returns>
		public static T Deserialize<T>(this IEnumerable<HashValue> hashEntries)
		{
			var type = typeof(T);
			var deserializer = _deserializers.ContainsKey(type) ? _deserializers[type] : _defaultDeserializer;
			return (T)deserializer.Deserialize<T>(hashEntries);
		}
	}
}
