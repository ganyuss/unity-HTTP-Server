using System.Net.Http;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    [PublicAPI]
    public class GetRouteAttribute : StaticMethodRouteAttribute
    {
        public override HttpMethod Method => HttpMethod.Get;
        public override string Path { get; }

        public GetRouteAttribute(string path) 
            => Path = path;

    }
}