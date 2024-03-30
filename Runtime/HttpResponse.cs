using System.Net;
using JetBrains.Annotations;

namespace UnityHttpServer
{
    [PublicAPI]
    public class HttpResponse
    {
        public IHttpResponseContent Content { get; } = new EmptyHttpResponse();
        public int StatusCode { get; }
        
        public HttpResponse(HttpStatusCode responseCode, [NotNull] IHttpResponseContent content)
            : this(responseCode)
        {
            Content = content;
        }

        public HttpResponse(HttpStatusCode responseCode)
        {
            StatusCode = (int) responseCode;
        }

        public HttpResponse(int statusCode, [NotNull] IHttpResponseContent content)
            : this(statusCode)
        {
            Content = content;
        }

        public HttpResponse(int statusCode)
        {
            StatusCode = statusCode;
        }
        
        public static implicit operator HttpResponse(HttpStatusCode statusCode) => new HttpResponse(statusCode);

        internal void Apply(HttpListenerResponse listenerResponse)
        {
            listenerResponse.StatusCode = StatusCode;
            
            listenerResponse.ContentLength64 = Content.ContentSize;
            listenerResponse.ContentEncoding = Content.ContentEncoding;
            listenerResponse.ContentType = Content.ContentType;
            
            Content.WriteToStreamAsync(listenerResponse.OutputStream);
            listenerResponse.OutputStream.Close();
        }
    }
}