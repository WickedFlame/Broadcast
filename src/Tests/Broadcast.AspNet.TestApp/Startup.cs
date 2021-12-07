using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Broadcast.Dashboard;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Broadcast.AspNet.Test.Startup))]

namespace Broadcast.AspNet.Test
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseBroadcastServer(c =>
			{
				c.UseOptions(new Broadcast.Configuration.ProcessorOptions
				{
					ServerName = "Asp.Net"
				});
			});
			app.UseBroadcastDashboard();

			BackgroundTask.Recurring(() => Trace.WriteLine("Broadcast Server task set from Startup"), TimeSpan.FromSeconds(20));
			BackgroundTask.Recurring("Action", () => Trace.WriteLine("Broadcast task set from Startup"), TimeSpan.FromSeconds(30));
		}
	}
}