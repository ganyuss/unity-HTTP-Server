using System;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    /// <summary>
    /// This class is the base class for implementing static routes
    /// in a controller. If a request matches a route attribute, the
    /// method with this attribute will consume this request. 
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public abstract class ConsumerAttribute : Attribute
    {
        /// <summary>
        /// See <see cref="UnityHttpServer.Controller.IHttpRequestConsumer.Match(HttpRequest)">
        /// IHttpRequestConsumer.Match(HttpRequest)</see>.
        /// </summary>
        public abstract bool MatchRequest([NotNull] HttpRequest request);
    }
}