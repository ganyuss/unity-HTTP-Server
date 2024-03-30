using System;
using JetBrains.Annotations;

namespace UnityHttpServer.Routing
{
    [PublicAPI]
    [MeansImplicitUse]
    public abstract class RouteAttribute : Attribute
    {
        public abstract bool MatchRequest(HttpRequest request);
    }
}