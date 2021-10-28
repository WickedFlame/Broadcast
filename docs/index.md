---
layout: "default"
---
# Broadcast
Simple and easy to use background task processing and message queue for .NET  
  
Broadcast is a simple implementation for processing and scheduling tasks in the background without blocking the main thread.  
Broadcast helps implement the Mediator or CQRS (Command- and Queryhandling) patterns easily.
  
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

[Changelog](changelog)