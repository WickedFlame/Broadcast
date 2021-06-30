---
layout: "default"
---
# Broadcast Console

The console displays as an overlay that shows live metrics of the servers and tasks.  
The console can only be used when Broadcast is already configured.  

## Configuration
### ASP.NET Core
In Startup.cs extend the Configure-Method to add the Broadcast dashboard to the ApplicationBuilder.
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseBroadcastDashboard();
    ...
}
```
Broadcast has to be configured in the ConfigureService before adding the dashboard.  

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
	app.UseBroadcastDashboard();

}
```

## Display the console
The console is added to the page by adding some JavaScript files to the page. These can easily be added with a call to BroadcastConsole.AppendConsoleIncludes().

```csharp
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Broadcast Console demo</title>
    <link rel="stylesheet" href="~/css/site.css" />

    @BroadcastConsole.AppendConsoleIncludes()
</head>
```

If you are using an older version of MVC you may need to wrap the command in a Html.Raw to ensure the console is not Html-Encoded
```csharp
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Broadcast Console demo</title>
    <link rel="stylesheet" href="~/css/site.css" />

    @Html.Raw(@BroadcastConsole.AppendConsoleIncludes())
</head>
```

### Resulting output
BroadcastConsole.AppendConsoleIncludes() adds the following to the page
```javascript
<link rel="stylesheet" href="/broadcast/css/broadcast-console"/>
<script type="text/javascript">
  var consoleConfig = {
    pollUrl: "/broadcast/dashboard/metrics",
    pollInterval: 2000
  };
</script>
<script type='module' async src="/broadcast/js/broadcast-console"></script>
```
