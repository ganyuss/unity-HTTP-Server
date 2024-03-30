using System.Net.Http;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    /// <summary>
    /// Marks a method to respond to HTTP PUT requests
    /// </summary>
    [PublicAPI]
    public class PutConsumerAttribute : StaticMethodConsumerAttribute
    {
        public override HttpMethod Method => HttpMethod.Put;
        public override string Path { get; }

        /// <summary>
        /// Marks a method to respond to HTTP PUT requests
        /// </summary>
        /// <param name="path">The route the method will respond to</param>
        public PutConsumerAttribute(string path) 
            => Path = path;
    }
}