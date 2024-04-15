using System.Threading.Tasks;
using JetBrains.Annotations;

namespace UnityHttpServer.Controller
{
    /// <summary>
    /// Represents an HTTP request consumer, often declared by a controller.
    /// </summary>
    [PublicAPI]
    public interface IHttpRequestConsumer
    {
        /// <summary>
        /// Return <c>true</c> if the consumer can consume the given HTTP request,
        /// <c>false</c> otherwise. <br /><br />
        /// If <c>true</c> is returned, <see cref="Consume(HttpRequest)"/> will
        /// be called.
        /// </summary>
        /// <param name="request">The request to consume</param>
        /// <returns></returns>
        public bool Match([NotNull] HttpRequest request);
        
        /// <summary>
        /// Consume the HTTP request, and return an HTTP response to send
        /// back to the client.
        /// </summary>
        /// <param name="request">The request to consume</param>
        /// <returns></returns>
        [NotNull] 
        public Task<HttpResponse> ConsumeAsync([NotNull] HttpRequest request);
    }
}