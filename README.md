# Unity HTTP Server

This package lets you create a simple HTTP server, to listen for and answer to HTTP requests. 

## Features

This package lets you create static and dynamic endpoints, to handle HTTP requests. 

You can respond to this request with any type of content you would need.

## Getting Started

You need to create you own controller class, implementing different HTTP request consumers (see below). You then need to pass an instance of this controller class to the `HttpServer` constructor, and start the server. 

You can implement consumers in two different ways:
1. Statically, by adding an implementer of `ConsumerAttribute` to any of your controller's method, like `GetConsumerAttribute`.
2. Dynamically, by implementing `IDynamicConsumerProvider` in your controller.

Method with a `ConsumerAttribute` can take as parameter either nothing, or a `HttpRequest`. They can return `void`, or an `HttpResponse`.

If no consumer is found for a given request, an empty `404` response will be returned. If a consumer's `Consume` method throws an exception, an `500` empty response will be returned. If a static consumer is based off a void method, an empty `200` after the method has ran.

## Examples

### Usage of the server

Starting the server:
```csharp
var myController = new MyHttpController();

// This will create a server with default "Debug" logging, for custom logger, you can also 
// provide an ILogger object to the constructor. Explicitely providing null will disable logging.
// ***PROVIDING A NULL LOGGER WILL PREVENT ALL EXCEPTIONS FROM BEING LOGGED***
var server = new HttpServer(myController, port: 8080);

// Now the server starts listening on the given port (8080)
server.Start();
```

Stopping the server:
```csharp
// This will stop the server from listening
server.Stop();

// In order to properly close the server, you will need to dispose of it
server.Dispose();
```

### Creating a controller class

Using static consumers:
```csharp
public class MyHttpController {

    // This method will be callled whenever the root of the server will be requested.
    // Example: "http://localhost:8080/"
    [GetConsumer("/")]
    public HttpResponse Index() {
        // This returns a response with code 200
        return new HttpResponse(HttpStatusCode.OK)
        {
            // With an HTML body greeting the user
            Content = new StringHttpResponseContent("<html><body>Hello people!<br />" 
                                                    + Time.realtimeSinceStartup + "</body></html>",
                System.Net.Mime.MediaTypeNames.Text.Html),
        };
    }
    
    // methods can take the HTTP request as parameter
    [GetConsumer("/my-processing-endpoint")]
    public HttpResponse Process(HttpRequest request) {
        ProcessRequest(request);
        
        // status code are converted automatically to empty HTTP responses
        return HttpStatusCode.OK;
    }
    
    // All the end points have to be defined in the same controller class for a given server
    [GetConsumer("/my-page")]
    public HttpResponse MyPage() {
        return new HttpResponse(HttpStatusCode.OK)
        {
            Content = ...,
        };
    }
}
```

Using dynamic consumers:
```csharp
public class MyHttpController : IDynamicConsumerProvider {

    [GetConsumer("/")]
    public HttpResponse Index() {
        return new HttpResponse(...);
    }
    
    [GetConsumer("/my-page")]
    public HttpResponse MyPage() {
        return new HttpResponse(...);
    }
    
    // This methods provides request consumers dynamically. It will be called
    // every time a new request is received.
    public IEnumerable<IHttpRequestConsumer> GetDynamicMethods()
    {
        // This new here on every request is very inefficient. It is here for demonstration
        // purposes, but in a real scenario you would rather have a list as an instance property, 
        // that you would update dynamically
        return new[]
        {
            // dynamic methods are matched after the static ones, so they can be used
            // as fallback
            new HttpRequestConsumer(
                // This one for example will be executed for every DELETE HTTP request,
                // that are not matched to any other static consumer.
                matchDelegate: request => request.Method == HttpMethod.Delete,
                consumeDelegate: request => new HttpResponse(HttpStatusCode.MethodNotAllowed)
                {
                    Content = new StringHttpResponseContent($"Method {HttpMethod.Delete} not allowed")
                }),
            
            // This one will be matched last, as it is the last one of the dynamic 
            // consumers
            new HttpRequestConsumer(
                // It will match all the incomming requests
                matchDelegate: request => true,
                // And return a 404 response
                consumeDelegate: request => new HttpResponse(HttpStatusCode.NotFound)
                {
                    Content = new StringHttpResponseContent($"Content {request.Uri.AbsolutePath} cannot be found")
                })
        };
    }
    
}
```