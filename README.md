# WickedFlame Broadcast
------------------------------
[![Build Status](https://img.shields.io/travis/com/WickedFlame/broadcast/master.svg?label=Travis-CI&style=for-the-badge)](https://app.travis-ci.com/github/WickedFlame/Broadcast)
[![Build status](https://img.shields.io/appveyor/build/chriswalpen/broadcast/master?label=Master&logo=appveyor&style=for-the-badge)](https://ci.appveyor.com/project/chriswalpen/broadcast/branch/master)
[![Build status](https://img.shields.io/appveyor/build/chriswalpen/broadcast/dev?label=Dev&logo=appveyor&style=for-the-badge)](https://ci.appveyor.com/project/chriswalpen/broadcast/branch/dev)
[![NuGet Version](https://img.shields.io/nuget/v/broadcast.svg?style=for-the-badge&label=Latest)](https://www.nuget.org/packages/broadcast/)
[![NuGet Version](https://img.shields.io/nuget/vpre/broadcast.svg?style=for-the-badge&label=RC)](https://www.nuget.org/packages/broadcast/)

  
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=WickedFlame_Broadcast&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=WickedFlame_Broadcast)
  
Simple and easy to use background task processing and message queue for .NET  
  
Broadcast is a simple implementation for processing and scheduling tasks in the background without blocking the main thread.  
Broadcast helps implement the Mediator or CQRS (Command- and Queryhandling) patterns easily.
  
Visit [https://wickedflame.github.io/Broadcast/](https://wickedflame.github.io/Broadcast/) for the full documentation.
  
## Installation
Broadcast is available as a NuGet package
```
PM> Install-Package Broadcast
```
  
After installation setup the Processingserver in Startup.cs with a dashboard if desired
```
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddBroadcast();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
	app.UseBroadcastServer();
	app.UseBroadcastDashboard();
}
```
  
## Usage
### Background Task processing
Processing a task in a async queue using the default Broadcaster
```csharp
BackgroundTaskClient.Send(() => Trace.WriteLine("This is a basic task"));
```
  
Processing a task with a custom Broadcaster instance
```csharp
var broadcaster = new Broadcaster();
broadcaster.Send(() => Trace.WriteLine("This is a basic task"));
```

### Scheduleed Tasks
Schedule a task in a async queue using the default Broadcaster
```csharp
BackgroundTaskClient.Schedule(() => Console.WriteLine("test"), TimeSpan.FromMinutes(1));
```
  
Schedule a task with a custom Broadcaster instance
```csharp
var broadcaster = new Broadcaster();
broadcaster.Schedule(() => Console.WriteLine("test"), TimeSpan.FromMinutes(1));
```

### Recurring Tasks
Add a recurring task to be processed in a async queue using the default Broadcaster
```csharp
BackgroundTaskClient.Recurring("recurring", () => service.Recurring(DateTime.Now.ToString("o")), TimeSpan.FromMinutes(15));
```
  
Add a recurring task to be processed with a custom Broadcaster instance
```csharp
var broadcaster = new Broadcaster();
broadcaster.Recurring("recurring", () => service.Recurring(DateTime.Now.ToString("o")), TimeSpan.FromMinutes(15));
```
