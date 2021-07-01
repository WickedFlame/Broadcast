---
layout: "default"
---
# Broadcast Setup

Broadcast can be used as per instance or as a Server in a Asp.Net environment.  

## Configuration
### ASP.NET Core
In Startup.cs extend the ConfigureServices- and the Configure-Method to add Broadcast as a Application service.  
The servers connect to and interact with the configured ITaskStore. It is possible to start multiple servers that connect to the same ITaskStore.  
```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddBroadcast(c => c.UseTaskStore(new TaskStore()));
  ...
}
```
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseBroadcastServer(new Options
		{
			ServerName = "Asp.Net Core"
		});
    ...
}
```
In Asp.Net Core it is possible to start multiple server by using app.UseBroadcastServer. It is recomended that each server has an individual name.  

### ASP.NET
In Startup.cs extend the Configure-Method to add the Broadcast dashboard to the AppBuilder.
```csharp
public void Configuration(IAppBuilder app)
{
	app.UseBroadcastServer(c =>
	{
		c.UseOptions(new Broadcast.Configuration.Options
		{
			ServerName = "Asp.Net"
		});
	});
}
```

