using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityHttpServer.Controller;

namespace UnityHttpServer
{
    /// <summary>
    /// Listen for HTTP requests on the given port, and forwards requests
    /// to the <see cref="IHttpRequestConsumer"/>s of the given
    /// controller.  
    /// </summary>
    [PublicAPI]
    public class HttpServer : IDisposable
    {
        private readonly HttpListener HttpListener = new HttpListener();
        private readonly IHttpControllerWrapper _controllerWrapper;
        [CanBeNull] 
        private readonly ILogger Logger;

        private readonly int Port;
        
        public bool Running { get; private set; }
        [NotNull]
        private TaskCompletionSource<bool> RunningTask = new TaskCompletionSource<bool>();
        
        public HttpServer(object controller, int port) : this(controller, port, Debug.unityLogger)
        {
        }
        
        internal HttpServer(IHttpControllerWrapper controllerWrapper, int port)
            : this(controllerWrapper, port, Debug.unityLogger)
        {
        }

        public HttpServer(object controller, int port, [CanBeNull] ILogger logger)
            : this(new HttpControllerWrapperWrapper(controller, logger), port, logger)
        {
        }
        
        internal HttpServer(IHttpControllerWrapper controllerWrapper, int port, [CanBeNull] ILogger logger)
        {
            _controllerWrapper = controllerWrapper;
            Port = port;
            HttpListener.Prefixes.Add($"http://*:{port}/");
            Logger = logger;
        }

        /// <summary>
        /// Allows the server to receive the incoming requests, on the given port.
        /// </summary>
        public void Start()
        {
            Running = true;
            HttpListener.Start();
            
            Logger?.Log($"Start listening on port {Port}...");

            RunningTask = new TaskCompletionSource<bool>();
            ListenAsync()
                .ListenForErrors();
        }

        private async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    var runningTask = RunningTask.Task;
                    var getContextTask = HttpListener.GetContextAsync();

                    var completedTask = await Task.WhenAny(runningTask, getContextTask);

                    if (completedTask == runningTask)
                        // Server stopped
                        return;
                    
                    var httpListenerContext = getContextTask.Result;
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
            
            HttpResponse response;
            var request = new HttpRequest(httpListenerContext.Request);
            
            try
            {
                if (_controllerWrapper.TryConsume(request, out response))
                {
                    response.Apply(httpListenerContext.Response);
                    Logger?.Log($"Consumer found, code {response.StatusCode}");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger?.Log(LogType.Exception, e);
                response = HttpStatusCode.InternalServerError;
                response.Apply(httpListenerContext.Response);
                return;
            }
            
            Logger?.Log(LogType.Warning, "No consumer found, returning 404");
            response = HttpStatusCode.NotFound;
            response.Apply(httpListenerContext.Response);
        }


        public void Stop()
        {
            RunningTask.SetResult(true);
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