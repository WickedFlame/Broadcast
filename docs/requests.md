---
title: Requests
layout: "default"
nav_order: 4
---
## Requests
```csharp
class Request : IRequest { }

class RequestHandler : IRequestHandler<Request>
{
    public void Handle(Request request)
    {
        ...
    }
}

...
var requestHandler = new RequestHandler();

// create the RequestPublisher
var publisher = new RequestPublisher<Request>(requestHandler);

// pass the request to be published
publisher.Handle(new Request(5));
```