---
layout: "default"
---
# Broadcast

Background task processing and message queue for .NET

## Background Task processing
```csharp
var broadcaster = new Broadcaster();
broadcaster.Send(() => Trace.WriteLine("This is a basic task"));
```

## Scheduleed Tasks
```csharp
var broadcaster = new Broadcaster();
broadcaster.Schedule(() => Console.WriteLine("test"), TimeSpan.FromMinutes(1));
```

## Message Queue
### Notification
```csharp
class Message : INotification { }

class NotificationHandler : INotificationTarget<Message>
{
    public void Handle(Message notification)
    {
        ...
    }
}

var broadcaster = new Broadcaster();

broadcaster.RegisterHandler(notificationHandler);

broadcaster.Send(new Message(5));
```


[Changelog](changelog)