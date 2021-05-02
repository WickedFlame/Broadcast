using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Broadcast
{
	public static class BroadcastServiceCollectionExtensions
	{
		public static IServiceCollection AddBroadcast([NotNull] this IServiceCollection services, Action<ServerSetup> config)
		{
			var serverSetup = new ServerSetup();

			config(serverSetup);

			var options = serverSetup.Resolve<Options>() ?? new Options();
			services.TryAddSingletonChecked(_ => options);

			var storage = serverSetup.Resolve<IStorage>() ?? new InmemoryStorage();

			var store = serverSetup.Resolve<ITaskStore>() ?? new TaskStore(options, storage);
			services.TryAddSingletonChecked(_ => store);

			return services;
		}

		private static void TryAddSingletonChecked<T>(this IServiceCollection serviceCollection, Func<IServiceProvider, T> implementationFactory) where T : class
		{
			serviceCollection.TryAddSingleton<T>(serviceProvider =>
			{
				if (serviceProvider == null)
				{
					throw new ArgumentNullException(nameof(serviceProvider));
				}

				// ensure the configuration was performed
				//serviceProvider.GetRequiredService<IGlobalConfiguration>();

				return implementationFactory(serviceProvider);
			});
		}
	}

	public interface IServerSetup
	{
	}

	public class ServerSetup : IServerSetup
	{
		/// <summary>
		/// The context for the configuration
		/// </summary>
		internal Dictionary<string, object> Context { get; } = new Dictionary<string, object>();
	}





	/// <summary>
	/// Extensions for the <see cref="ServerSetup"/>
	/// </summary>
	public static class ServerSetupExtensions
	{
		/// <summary>
		/// Setup the ProcessingServer with the help of the PipelineBuilder
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="store"></param>
		/// <returns></returns>
		public static ServerSetup UseTaskStore(this ServerSetup setup, ITaskStore store)
		{
			setup.Register(store);

			return setup;
		}

		/// <summary>
		/// Set the options that are used for the Server
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static ServerSetup UseOptions(this ServerSetup setup, Options options)
		{
			setup.Register(options);
			return setup;
		}

		/// <summary>
		/// Register a item in the <see cref="ServerSetup"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static ServerSetup Register<T>(this ServerSetup setup, T item)
		{
			setup.Context[typeof(T).Name] = item;

			return setup;
		}

		/// <summary>
		/// Resolve a item from the <see cref="ServerSetup"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <returns></returns>
		public static T Resolve<T>(this ServerSetup setup)
		{
			var key = typeof(T).Name;
			if (setup.Context.ContainsKey(key))
			{
				return (T)setup.Context[key];
			}

			return default(T);
		}
	}
}
