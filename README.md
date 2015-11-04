# WickedFlame Broadcast
------------------------------
[![Build Status](https://travis-ci.org/WickedFlame/Broadcast.svg?branch=master)](https://travis-ci.org/WickedFlame/Broadcast)
[![Build status](https://ci.appveyor.com/api/projects/status/ybdr1oa8p4x8h9h6/branch/master?svg=true)](https://ci.appveyor.com/project/chriswalpen/broadcast/branch/master)


A simple and lightweight in-process fire and forget, request/response message queue.  
Broadcast is a simple implementation for processing queued tasks and notifications synchronous and asynchronous.  
Broadcast helps implement the Mediator and CQRS (Command- and Queryhandling) patterns easily.

## Installation
------------------------------
Broadcast can be installed from [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget) through the package manager console:  

    PM > Install-Package Broadcast

# Examples
------------------------------
## Broadcaster - Notification and Taskprocessing
### Fire and forget task processing
```csharp
// Broadcaster is usualy resolved through DependencyInjection
// Synchronous task processing
IBroadcaster broadcaster = new Broadcaster();
broadcaster.Send(() => Trace.WriteLine("This is a basic synchronous processed task"));

// Asynchronous task processing with a background thread
IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Background);
broadcaster.Send(() => Trace.WriteLine("All tasks are processed in a backgroundthread"));

// Asynchronous task processing for all tasks
IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Async);
broadcaster.Send(() => Trace.WriteLine("Each task is processed asynchronously in a separate thread"));
```

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
IBroadcaster broadcaster = new Broadcaster();

// register different handlers
broadcaster.RegisterHandler(notificationHandler);
broadcaster.RegisterHandler<Message>(delegateHandler.Handle);
broadcaster.RegisterHandler<Message>(a => expressionHandler = a.ID);

// publish a message to the handlers
broadcaster.Send(new Message(5));
```

## Mediator - Notification
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
mediator.RegisterHandler<Message>(delegateHandler.Handle);
mediator.RegisterHandler<Message>(a => expressionHandler = a.ID);

// publish a message to the handlers
mediator.Send(new Message(5));
```

## Requests
```csharp
// dummy implementation of a Request (Command/Query)
class Request : IRequest { }
// dummy implementation of a RequestHandler (Commandhandler/Queryhandler)
class RequestHandler : IRequestHandler<Request>
{
    public void Handle(Request request)
    {
        ...
    }
}

// the Requestpublisher is usualy resolved through DependencyInjection and the RequestHandler is passed/resolved with ConstructorInjection
var requestHandler = new RequestHandler();

// create the RequestPublisher
var publisher = new RequestPublisher<Request>(requestHandler);

// pass the request to be published
publisher.Handle(new Request(5));
```