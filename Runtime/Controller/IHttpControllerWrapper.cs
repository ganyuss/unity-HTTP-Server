namespace UnityHttpServer.Controller
{
    internal interface IHttpControllerWrapper
    {
        public bool TryConsume(HttpRequest request, out HttpResponse response);
    }
}