namespace UnityHttpServer.Controller
{
    internal interface IHttpController
    {
        public bool TryConsume(HttpRequest request, out HttpResponse response);
    }
}