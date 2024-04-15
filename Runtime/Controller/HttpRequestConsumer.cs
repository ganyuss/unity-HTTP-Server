using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace UnityHttpServer.Controller
{
    /// <summary>
    /// Simple implementation of <see cref="IHttpRequestConsumer"/>.
    /// </summary>
    [PublicAPI]
    public class HttpRequestConsumer : IHttpRequestConsumer
    {
        private readonly Func<HttpRequest, bool> MatchDelegate;
        private readonly Func<HttpRequest, HttpResponse> ConsumeDelegate;

        public HttpRequestConsumer([NotNull] Func<HttpRequest, bool> matchDelegate, 
            [NotNull] Func<HttpRequest, HttpResponse> consumeDelegate)
        {
            MatchDelegate = matchDelegate;
            ConsumeDelegate = consumeDelegate;
        }


        public bool Match(HttpRequest request)
        {
            return MatchDelegate.Invoke(request);
        }

        public Task<HttpResponse> ConsumeAsync(HttpRequest request)
        {
            return Task.FromResult(ConsumeDelegate.Invoke(request));
        }
    }
}