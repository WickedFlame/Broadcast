using System;
using System.Collections.Generic;
using System.Linq;
using Broadcast.Composition;
using Broadcast.EventSourcing;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A serializer or deserializer for serializing and deserializing a <see cref="HashValue"/> to a <see cref="BroadcastTask"/>
	/// </summary>
	public class BroadcastTaskSerializer : ISerializer, IDeserializer
	{
		/// <summary>
		/// Serialize a <see cref="BroadcastTask"/> to a list of <see cref="HashValue"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public IEnumerable<HashValue> Serialize(object obj)
		{
			if (obj is BroadcastTask task)
			{
				var hashes = new List<HashValue>
				{
					new HashValue("Id", task.Id),
					new HashValue("Name", task.Name),
					new HashValue("State", task.State.ToString()),
					new HashValue("Type", $"{task.Type.FullName}, {task.Type.Assembly.GetName().Name}"),
					new HashValue("IsRecurring", task.IsRecurring.ToString()),
					new HashValue("Time", task.Time?.ToString())
				};

				foreach (var state in task.StateChanges)
				{
					hashes.Add(new HashValue($"StateChanges:{state.Key.ToString()}", state.Value.ToString("o")));
				}

				hashes.Add(new HashValue("Method", task.Method.Name));

				var parameterTypes = task.Method.GetParameters().Select(x => $"{x.ParameterType.FullName}, {x.ParameterType.Assembly.GetName().Name}");
				var cnt = 0;
				foreach (var paramType in parameterTypes)
				{
					hashes.Add(new HashValue($"ArgsType:{cnt}", paramType));
					hashes.Add(new HashValue($"ArgsValue:{cnt}", task.Args[cnt].ToString()));
					cnt = cnt + 1;
				}

				return hashes.ToArray();
			}

			return null;
		}

		/// <summary>
		/// Deserialize a list of <see cref="HashValue"/> to a <see cref="BroadcastTask"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="hashEntries"></param>
		/// <returns></returns>
		public object Deserialize<T>(IEnumerable<HashValue> hashEntries)
		{
			if (!hashEntries.Any())
			{
				throw new ArgumentException(nameof(hashEntries));
			}

			if (typeof(T) != typeof(BroadcastTask))
			{
				return null;
			}

			var task = new BroadcastTask
			{
				Id = hashEntries.FirstOrDefault(h => h.Name == "Id")?.Value,
				Name = hashEntries.FirstOrDefault(h => h.Name == "Name")?.Value,
				State = TypeConverter.Convert<TaskState>(hashEntries.FirstOrDefault(h => h.Name == "State")?.Value),
				Type = TypeConverter.Convert<Type>(hashEntries.FirstOrDefault(h => h.Name == "Type")?.Value),
				IsRecurring = TypeConverter.Convert<bool>(hashEntries.FirstOrDefault(h => h.Name == "IsRecurring")?.Value),
				Time = TypeConverter.Convert<TimeSpan?>(hashEntries.FirstOrDefault(h => h.Name == "Time")?.Value)
			};

			foreach (var state in hashEntries.Where(h => h.Name.StartsWith("StateChanges:")))
			{
				var name = state.Name.Substring(13);
				task.StateChanges[TypeConverter.Convert<TaskState>(name)] = TypeConverter.Convert<DateTime>(state.Value);
			}

			var method = hashEntries.FirstOrDefault(h => h.Name == "Method")?.Value;
			var argumentTypes = new List<Type>();
			var arguments = new List<object>();
			for (var i = 0; i < hashEntries.Count(h => h.Name.StartsWith("ArgsType:")); i++)
			{
				var type = TypeConverter.Convert<Type>(hashEntries.FirstOrDefault(h => h.Name == $"ArgsType:{i}")?.Value);
				argumentTypes.Add(type);
				arguments.Add(TypeConverter.Convert(type, hashEntries.FirstOrDefault(h => h.Name == $"ArgsValue:{i}")?.Value));
			}

			if (task.Type == null || string.IsNullOrEmpty(method))
			{
				throw new InvalidOperationException("Type or Method is null for the Task");
			}

			task.Args = arguments.ToArray();
			task.Method = task.Type.GetNonOpenMatchingMethod(method, argumentTypes.ToArray());

			return task;
		}
	}
}
