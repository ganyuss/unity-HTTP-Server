using System.Threading.Tasks;
using JetBrains.Annotations;

namespace UnityHttpServer.Controller
{
    internal interface IHttpControllerWrapper
    {
        [ItemCanBeNull]
        public Task<HttpResponse> TryConsumeAsync(HttpRequest request);
    }
}