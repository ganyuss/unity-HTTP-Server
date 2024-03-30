using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace UnityHttpServer
{
    [PublicAPI]
    public class HttpResponse
    {
        public int StatusCode { get; }
        public string StatusDescription => Enum.GetName(typeof(HttpStatusCode), StatusCode) ?? string.Empty;
        
        public List<Cookie> Cookies { get; } = new List<Cookie>();
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        
        public IHttpResponseContent Content { get; set; } = new EmptyHttpResponse();
        
        [CanBeNull] 
        public string RedirectionLocation { get; set; }
        public bool KeepAlive { get; set; }
        
        public HttpResponse(HttpStatusCode responseCode)
        {
            StatusCode = (int) responseCode;
        }

        public HttpResponse(int statusCode)
        {
            StatusCode = statusCode;
        }
        
        public static implicit operator HttpResponse(HttpStatusCode statusCode) => new HttpResponse(statusCode);

        internal void Apply(HttpListenerResponse listenerResponse)
        {
            listenerResponse.StatusCode = StatusCode;
            listenerResponse.StatusDescription = StatusDescription;
            
            foreach (var cookie in Cookies) 
                listenerResponse.SetCookie(cookie);
            
            foreach (var header in Headers) 
                listenerResponse.AddHeader(header.Key, header.Value);
            
            listenerResponse.RedirectLocation = RedirectionLocation;
            listenerResponse.KeepAlive = KeepAlive;

            listenerResponse.ContentLength64 = Content.ContentSize;
            listenerResponse.ContentEncoding = Content.ContentEncoding;
            listenerResponse.ContentType = Content.ContentType;
            
            Content.WriteToStreamAsync(listenerResponse.OutputStream);
            listenerResponse.OutputStream.Close();
        }
    }
}