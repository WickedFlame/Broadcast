---
layout: "default"
---
# Broadcast Console

The console displays as an overlay that shows live metrics of the servers and tasks.  

## Configuration
### ASP .NET Core
In Startup.cs extend the Configure-Method to add the Broadcast dashboard
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseBroadcastDashboard();
    ...
}
```

## Display the console
The console is added to the head of the layout razorpage by adding a call to  
BroadcastConsole.AppendConsoleIncludes().

```csharp
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Broadcast Console demo</title>
    <link rel="stylesheet" href="~/css/site.css" />

    @BroadcastConsole.AppendConsoleIncludes();
</head>
```