using System.Net;
using JetBrains.Annotations;

namespace UnityHttpServer.Controller
{
    [PublicAPI]
    public interface IControllerMethod
    {
        public bool Match(HttpRequest request);
        public HttpResponse Consume(HttpRequest request);
    }
}