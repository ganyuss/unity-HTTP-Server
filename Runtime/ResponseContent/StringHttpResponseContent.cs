using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnityHttpServer
{
    public class StringHttpResponseContent : IHttpResponseContent
    {
        public string Content { get; }

        public long ContentSize => Encoding.UTF8.GetBytes(Content).Length;
        public string ContentType { get; }
        
        public StringHttpResponseContent(string content, string contentType = System.Net.Mime.MediaTypeNames.Text.Plain)
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