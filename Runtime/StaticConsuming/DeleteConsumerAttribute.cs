using System.Net.Http;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    /// <summary>
    /// Marks a method to respond to HTTP DELETE requests
    /// </summary>
    [PublicAPI]
    public class DeleteConsumerAttribute : StaticMethodConsumerAttribute
    {
        public override HttpMethod Method => HttpMethod.Delete;
        public override string Path { get; }

        /// <summary>
        /// Marks a method to respond to HTTP DELETE requests
        /// </summary>
        /// <param name="path">The route the method will respond to</param>
        public DeleteConsumerAttribute(string path) 
            => Path = path;
    }
}