using System.Net.Http;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    /// <summary>
    /// Marks a method to respond to HTTP POST requests
    /// </summary>
    [PublicAPI]
    public class PostConsumerAttribute : StaticMethodConsumerAttribute
    {
        public override HttpMethod Method => HttpMethod.Post;
        public override string Path { get; }

        /// <summary>
        /// Marks a method to respond to HTTP POST requests
        /// </summary>
        /// <param name="path">The route the method will respond to</param>
        public PostConsumerAttribute(string path) 
            => Path = path;
    }
}