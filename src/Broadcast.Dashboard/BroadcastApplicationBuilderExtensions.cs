using System;
using System.Diagnostics.CodeAnalysis;
using Broadcast.Configuration;
using Broadcast.Dashboard;
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
		public static IApplicationBuilder UseBroadcastServer(this IApplicationBuilder app, Options options = null)
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
			var storage = services.GetRequiredService<ITaskStore>();
			options ??= services.GetService<Options>() ?? new Options();

			var processor = new TaskProcessor(storage, options);
			var scheduler = new Scheduler();

			var server = new Broadcaster(storage, processor, scheduler, options);

			BackgroundTaskClient.Setup(() => new BroadcastingClient(storage));

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
			var storage = app.ApplicationServices.GetRequiredService<ITaskStore>();

			app.Map(new PathString(pathMatch), x => x.UseMiddleware<DashboardMiddleware>(routes, storage));

			return app;
		}
	}
}
