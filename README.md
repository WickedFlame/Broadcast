# WickedFlame Broadcast
------------------------------

A simple fire and forget request/response and messaging commponent. Supports a basic implementation of synchronous and async request/response (commands/queries) and notifications

## Installation
------------------------------
Broadcast can be installed from [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget) through the package manager console:  

    PM > Install-Package Broadcast

# Examples
------------------------------
## Mediator / Notification
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
mediator.Publish(new Message(5));
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