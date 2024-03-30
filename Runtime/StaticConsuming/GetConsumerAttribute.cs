using System.Net.Http;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    /// <summary>
    /// Marks a method to respond to HTTP GET requests
    /// </summary>
    [PublicAPI]
    public class GetConsumerAttribute : StaticMethodConsumerAttribute
    {
        public override HttpMethod Method => HttpMethod.Get;
        public override string Path { get; }

        /// <summary>
        /// Marks a method to respond to HTTP GET requests
        /// </summary>
        /// <param name="path">The route the method will respond to</param>
        public GetConsumerAttribute(string path) 
            => Path = path;
    }
}