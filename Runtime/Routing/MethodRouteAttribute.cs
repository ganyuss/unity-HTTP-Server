using System.Net.Http;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    [PublicAPI]
    public abstract class StaticMethodConsumerAttribute : ConsumerAttribute
    {
        public abstract HttpMethod Method { get; }
        public abstract string Path { get; }
        
        public override bool MatchRequest(HttpRequest request)
        {
            return request.Method == Method
                   && request.Uri.AbsolutePath == Path;
        }
    }
}