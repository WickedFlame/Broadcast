using System;
using System.Collections.Generic;

namespace Broadcast
{
	/// <summary>
	/// Server configuration helper
	/// </summary>
	public class ServerConfiguration
	{
		internal Dictionary<Type, object> Context { get; } = new Dictionary<Type, object>();

		internal T Get<T>()
		{
			if (Context.ContainsKey(typeof(T)))
			{
				return (T)Context[typeof(T)];
			}

			return default(T);
		}

		internal void Add<T>(T item)
		{
			Context[typeof(T)] = item;
		}
	}
}
