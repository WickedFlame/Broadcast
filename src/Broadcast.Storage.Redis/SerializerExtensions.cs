using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Broadcast.EventSourcing;
using StackExchange.Redis;
using Broadcast.Composition;

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
			if (obj is BroadcastTask task)
			{
				var hashes = new List<HashEntry>
				{
					new HashEntry("Id", task.Id),
					new HashEntry("Name", task.Name),
					new HashEntry("State", task.State.ToString()),
					new HashEntry("Type", $"{task.Type.FullName}, {task.Type.Assembly.GetName().Name}"),
					new HashEntry("IsRecurring", task.IsRecurring.ToString()),
					new HashEntry("Time", task.Time?.ToString())
				};

				foreach (var state in task.StateChanges)
				{
					hashes.Add(new HashEntry($"StateChanges:{state.Key.ToString()}", state.Value.ToString("O")));
				}

				//hashes.Add(new HashEntry("Method", task.Method));
				hashes.Add(new HashEntry("Method", task.Method.Name));

				var parameterTypes = task.Method.GetParameters().Select(x => $"{x.ParameterType.FullName}, {x.ParameterType.Assembly.GetName().Name}");
				var cnt = 0;
				foreach (var paramType in parameterTypes)
				{
					hashes.Add(new HashEntry($"ArgsType:{cnt}", paramType));
					hashes.Add(new HashEntry($"ArgsValue:{cnt}", task.Args[cnt].ToString()));
					cnt = cnt + 1;
				}

				//for (var i = 0; i< task.Args.Count; i++)
				//{
				//	var type = task.Args[i].GetType();
				//	hashes.Add(new HashEntry($"Args:{i}:Type", $"{type.FullName}, {type.Assembly.GetName().Name}"));
				//	hashes.Add(new HashEntry($"Args:{i}:Value", task.Args[i].ToString()));
				//}

				return hashes.ToArray();
			}

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
			else if (obj is BroadcastTask task)
			{
				task.Id = hashEntries.FirstOrDefault(h => h.Name == "Id").Value;
				task.Name = hashEntries.FirstOrDefault(h => h.Name == "Name").Value;
				task.State = TypeConverter.Convert<TaskState>(hashEntries.FirstOrDefault(h => h.Name == "State").Value);
				task.Type = TypeConverter.Convert<Type>(hashEntries.FirstOrDefault(h => h.Name == "Type").Value);
				task.IsRecurring = TypeConverter.Convert<bool>(hashEntries.FirstOrDefault(h => h.Name == "IsRecurring").Value);
				task.Time = TypeConverter.Convert<TimeSpan?>(hashEntries.FirstOrDefault(h => h.Name == "Time").Value);

				foreach (var state in hashEntries.Where(h => h.Name.StartsWith("StateChanges:")))
				{
					var name = state.Name.ToString().Substring(13);
					task.StateChanges[TypeConverter.Convert<TaskState>(name)] = TypeConverter.Convert<DateTime>(state.Value);
				}

				var method = hashEntries.FirstOrDefault(h => h.Name == "Method").Value;
				var argumentTypes = new List<Type>();
				var arguments = new List<object>();
				for (var i = 0; i < hashEntries.Count(h => h.Name.StartsWith("ArgsType:")); i++)
				{
					var type = TypeConverter.Convert<Type>(hashEntries.FirstOrDefault(h => h.Name == $"ArgsType:{i}").Value);
					argumentTypes.Add(type);
					arguments.Add(TypeConverter.Convert(type, hashEntries.FirstOrDefault(h => h.Name == $"ArgsValue:{i}").Value));
				}

				task.Args = arguments.ToArray();
				task.Method = task.Type.GetNonOpenMatchingMethod(method, argumentTypes.ToArray());
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
