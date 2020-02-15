---
title: Mediator
layout: "default"
nav_order: 3
---
## Mediator
Broadcast provides an implementation of the Mediator pattern.
Simply register Handlers that implement the INotificationTarget<T> interface

### Notification
```csharp
// dummy implementation of a Notification (Event)
class Message : INotification { }
// dummy implementation of a Notificationhandler (EventHandler)
class NotificationHandler : INotificationTarget<Message>
{
    public void Handle(Message notification)
    {
        ...
    }
}

// Broadcaster is usualy resolved through DependencyInjection
var broadcaster = new Broadcaster();

// register different handlers
broadcaster.RegisterHandler(notificationHandler);

// publish a message to the handlers
broadcaster.Send(new Message(5));
```

### Mediator class
If a mediator is all that is needed, the same logic can be executed in a Mediator implementation. Basicaly it is the same as in the Broadcaster, but only containes the Notification implementation.
```csharp
// dummy implementation of a Notification (Event)
class Message : INotification { }
// dummy implementation of a Notificationhandler (EventHandler)
class NotificationHandler : INotificationTarget<Message>
{
    public void Handle(Message notification)
    {
        ...
    }
}

// Mediator is usualy resolved through DependencyInjection
IMediator mediator = new Mediator();

// register different handlers
mediator.RegisterHandler(notificationHandler);

// publish a message to the handlers
mediator.Send(new Message(5));
```

### Registration of Handlers
Thera are three different ways to register handlers
```csharp
broadcaster.RegisterHandler(notificationHandler);
```
```csharp
broadcaster.RegisterHandler<Message>(delegateHandler.Handle);
```
```csharp
broadcaster.RegisterHandler<Message>(a => expressionHandler = a.ID);
```