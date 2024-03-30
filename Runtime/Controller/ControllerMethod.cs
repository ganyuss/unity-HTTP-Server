using System;
using JetBrains.Annotations;

namespace UnityHttpServer.Controller
{
    [PublicAPI]
    public class ControllerMethod : IControllerMethod
    {
        private readonly Func<HttpRequest, bool> MatchDelegate;
        private readonly Func<HttpRequest, HttpResponse> ConsumeDelegate;

        public ControllerMethod([NotNull] Func<HttpRequest, bool> matchDelegate, 
            [NotNull] Func<HttpRequest, HttpResponse> consumeDelegate)
        {
            MatchDelegate = matchDelegate;
            ConsumeDelegate = consumeDelegate;
        }


        public bool Match(HttpRequest request)
        {
            return MatchDelegate.Invoke(request);
        }

        public HttpResponse Consume(HttpRequest request)
        {
            return ConsumeDelegate.Invoke(request);
        }
    }
}