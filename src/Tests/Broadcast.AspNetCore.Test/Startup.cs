using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broadcast.AspNetCore.Test
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			services.AddBroadcast(c => c.UseTaskStore(new TaskStore()));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			//app.UseBroadcastServer(new Broadcast.Configuration.Options(), new TaskStore());
			app.UseBroadcastServer();
			app.UseBroadcastServer(new Broadcast.Configuration.Options
			{
				ServerName = "test 1"
			});

			app.UseBroadcastDashboard();


			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});





			TaskServerClient.Recurring(() => Trace.WriteLine("Broadcast Server task set from Startup"), TimeSpan.FromSeconds(20));
			BackgroundTaskClient.Recurring("Action", () => Trace.WriteLine("Broadcast task set from Startup"), TimeSpan.FromSeconds(30));
		}
	}
}
