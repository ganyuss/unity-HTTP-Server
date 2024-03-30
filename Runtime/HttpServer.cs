using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityHttpServer.Controller;

namespace UnityHttpServer
{
    [PublicAPI]
    public class HttpServer : IDisposable
    {
        private readonly HttpListener HttpListener = new HttpListener();
        private readonly IHttpController _controller;
        [CanBeNull] 
        private readonly ILogger Logger;

        private readonly int Port;
        
        public bool Running { get; private set; }
    
        public HttpServer(object controller, int port) : this(controller, port, Debug.unityLogger)
        {
        }
        
        internal HttpServer(IHttpController controller, int port)
            : this(controller, port, Debug.unityLogger)
        {
        }

        internal HttpServer(object controller, int port, [CanBeNull] ILogger logger)
            : this(new HttpControllerWrapper(controller, logger), port, logger)
        {
        }
        
        internal HttpServer(IHttpController controller, int port, [CanBeNull] ILogger logger)
        {
            _controller = controller;
            Port = port;
            HttpListener.Prefixes.Add($"http://*:{port}/");
            Logger = logger;
        }

        public void Start()
        {
            Running = true;
            HttpListener.Start();
            
            Logger?.Log($"Start listening on port {Port}...");

            ListenAsync()
                .ListenForErrors();
        }

        private async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    var httpListenerContext = await HttpListener.GetContextAsync();
                    ConsumeRequest(httpListenerContext);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void ConsumeRequest(HttpListenerContext httpListenerContext)
        {
            Logger?.Log($"Request received for URI \"{httpListenerContext.Request.Url}\"");
            
            var request = new HttpRequest(httpListenerContext.Request);
            if (_controller.TryConsume(request, out var response))
            {
                response.Apply(httpListenerContext.Response);
                Logger?.Log($"Consumer found, code {response.StatusCode}");
                return;
            }
            
            Logger?.Log(LogType.Warning, "No consumer found, returning 404");
            response = HttpStatusCode.NotFound;
            response.Apply(httpListenerContext.Response);
        }


        public void Stop()
        {
            HttpListener.Stop();
            Running = false;
            
            Logger?.Log($"Stopped listening on port {Port}");
        }
    
        public void Dispose()
        {
            if (Running)
                Stop();
            
            HttpListener.Close();
        }
    }
}