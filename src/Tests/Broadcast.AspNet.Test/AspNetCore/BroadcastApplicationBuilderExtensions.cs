using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broadcast
{
	public static class BroadcastApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseBroadcastDashboard(
			[NotNull] this IApplicationBuilder app,
			[NotNull] string pathMatch = "/broadcast",
			Options options = null)
		{
			if (app == null) throw new ArgumentNullException(nameof(app));
			if (pathMatch == null) throw new ArgumentNullException(nameof(pathMatch));

			var services = app.ApplicationServices;
			
			var storage = services.GetRequiredService<ITaskStore>();
			options = options ?? services.GetService<Options>() ?? Options.Default;
			//options.TimeZoneResolver = options.TimeZoneResolver ?? services.GetService<ITimeZoneResolver>();

			var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();

			//app.Map(new PathString(pathMatch), x => x.UseMiddleware<AspNetCoreDashboardMiddleware>(storage, options, routes));

			return app;
		}

		public static IApplicationBuilder UseBroadcastServer([NotNull] this IApplicationBuilder app, Options options = null)
		{
			if (app == null) throw new ArgumentNullException(nameof(app));

			var services = app.ApplicationServices;
#if NETCOREAPP3_0
            var lifetime = services.GetRequiredService<IHostApplicationLifetime>();
#else
			var lifetime = services.GetRequiredService<IApplicationLifetime>();
#endif
			var storage = services.GetRequiredService<ITaskStore>();
			options ??= services.GetService<Options>() ?? Options.Default;

			var processor = new TaskProcessor(options);
			var scheduler = new Scheduler();

			var server = new Broadcaster(storage, processor, scheduler, options);

			BroadcastingClient.Setup(() => new BroadcastingClient(storage));

			//lifetime.ApplicationStopping.Register(() => server.SendStop());
			lifetime.ApplicationStopped.Register(() => server.Dispose());

			return app;
		}
	}
}
