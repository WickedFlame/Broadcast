using System;
using System.Diagnostics.CodeAnalysis;
using Broadcast.Configuration;
using Broadcast.Dashboard;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broadcast
{
	public static class BroadcastApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseBroadcastServer(this IApplicationBuilder app, ProcessorOptions options = null)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			var services = app.ApplicationServices;
#if NETCOREAPP3_1
			var lifetime = services.GetRequiredService<IHostApplicationLifetime>();
#else
			var lifetime = services.GetRequiredService<IApplicationLifetime>();
#endif
			// use TaskStore.Default if the services.AddBroadcast() is not configured
			var storage = services.GetService<ITaskStore>() ?? TaskStore.Default;
			options ??= services.GetService<ProcessorOptions>() ?? new ProcessorOptions();

			var processor = new TaskProcessor(storage, options);
			var scheduler = new Scheduler();

			var server = new Broadcaster(storage, processor, scheduler, options);

			BackgroundTask.Setup(() => new BroadcastingClient(storage));

			//lifetime.ApplicationStopping.Register(() => server.SendStop());
			lifetime.ApplicationStopped.Register(() => server.Dispose());

			return app;
		}

		public static IApplicationBuilder UseBroadcastDashboard(this IApplicationBuilder app, string pathMatch = "/broadcast")
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			if (pathMatch == null)
			{
				throw new ArgumentNullException(nameof(pathMatch));
			}

			DashboardOptions.Default.RouteBasePath = pathMatch.EnsureLeadingSlash();
			DashboardOptions.Default.TemplateParameters["%(RouteBasePath)"] = pathMatch.EnsureLeadingSlash();

			var routes = app.ApplicationServices.GetService<RouteCollection>() ?? Routes.RouteCollection;
			var storage = app.ApplicationServices.GetService<ITaskStore>();
			if (storage == null)
			{
				Console.WriteLine("Could not get registered ITaskStore. Using TaskStore.Default instead");
				storage = TaskStore.Default;
			}

			app.Map(new PathString(pathMatch), x => x.UseMiddleware<DashboardMiddleware>(routes, storage));

			return app;
		}
	}
}
