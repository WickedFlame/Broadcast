
# Broadcast Scheduler
Using the scheduler a task can be executed at a scheduled time
```csharp
var broadcaster = new Broadcaster();
broadcaster.Schedule(() => Console.WriteLine("test"), TimeSpan.FromMinutes(1));
```

Alternatively the task can be scheduled at a recurring interval
```csharp
var broadcaster = new Broadcaster();
broadcaster.Recurring(() => Console.WriteLine("test"), TimeSpan.FromMinutes(1));
```