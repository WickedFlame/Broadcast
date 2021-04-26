using System;
using System.Collections.Generic;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Processing;

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

	/// <summary>
	/// Extensionmethods for the <see cref="ServerConfiguration"/>
	/// </summary>
	public static class ServerSetupExtensions
	{
		/// <summary>
		/// Add a <see cref="IScheduler"/> to the server
		/// </summary>
		/// <param name="config"></param>
		/// <param name="scheduler"></param>
		/// <returns></returns>
		public static ServerConfiguration AddScheduler(this ServerConfiguration config, IScheduler scheduler)
		{
			config.Add(scheduler);
			return config;
		}

		/// <summary>
		/// Add a <see cref="ITaskProcessor"/> to the server
		/// </summary>
		/// <param name="config"></param>
		/// <param name="processor"></param>
		/// <returns></returns>
		public static ServerConfiguration AddProcessor(this ServerConfiguration config, ITaskProcessor processor)
		{
			config.Add(processor);
			return config;
		}

		/// <summary>
		/// Add a <see cref="ITaskStore"/> to the server
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="store"></param>
		/// <returns></returns>
		public static ServerConfiguration AddTaskStore(this ServerConfiguration setup, ITaskStore store)
		{
			setup.Add(store);
			return setup;
		}

		/// <summary>
		/// Add a <see cref="Options"/> to the server
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static ServerConfiguration AddOptions(this ServerConfiguration setup, Options options)
		{
			setup.Add(options);
			return setup;
		}
	}
}
