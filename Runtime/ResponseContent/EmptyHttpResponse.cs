using System.IO;
using System.Threading.Tasks;

namespace UnityHttpServer
{
    /// <summary>
    /// An empty <see cref="IHttpResponseContent"/>.
    /// </summary>
    public class EmptyHttpResponse : IHttpResponseContent
    {
        public long ContentSize => 0;
        public string ContentType => string.Empty;
        
        public async Task WriteToStreamAsync(Stream output)
        {
            await Task.CompletedTask;
        }
    }
}