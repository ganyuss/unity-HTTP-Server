using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace UnityHttpServer.ResponseContent
{
    /// <summary>
    /// An <see cref="IHttpResponseContent"/> wrapping a string.
    /// </summary>
    public class StringHttpResponseContent : IHttpResponseContent
    {
        public string Content { get; }

        public long ContentSize => Encoding.UTF8.GetBytes(Content).Length;
        public string ContentType { get; }
        
        public StringHttpResponseContent([NotNull] string content, [NotNull] string contentType = System.Net.Mime.MediaTypeNames.Text.Plain)
        {
            Content = content;
            ContentType = contentType;
        }

        public async Task WriteToStreamAsync(Stream output)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Content);
            await output.WriteAsync(buffer);
        }
    }
}