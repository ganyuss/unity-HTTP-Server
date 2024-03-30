using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace UnityHttpServer
{
    /// <summary>
    /// Represents the output of an HTTP request, sent back to the client.
    /// </summary>
    [PublicAPI]
    public class HttpResponse
    {
        /// <summary>
        /// The status code of the response.
        /// </summary>
        /// <seeAlso cref="HttpStatusCode"/>
        public int StatusCode { get; }
        /// <summary>
        /// The status description of the request, inferred from the <see cref="StatusCode"/>.
        /// </summary>
        public string StatusDescription => Enum.GetName(typeof(HttpStatusCode), StatusCode) ?? string.Empty;
        
        /// <summary>
        /// The cookies of the response.
        /// </summary>
        public List<Cookie> Cookies { get; } = new List<Cookie>();
        /// <summary>
        /// The headers of the response.
        /// </summary>
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        
        /// <summary>
        /// The content of the response
        /// </summary>
        public IHttpResponseContent Content { get; set; } = new EmptyHttpResponse();
        
        /// <summary>
        /// If specified, will set the HTTP "Location" header of the response.
        /// Used for redirections, usually with <see cref="HttpStatusCode.Redirect">
        /// HttpStatusCode.Redirect</see>.
        /// </summary>
        [CanBeNull] 
        public string RedirectionLocation { get; set; }
        
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

            listenerResponse.ContentLength64 = Content.ContentSize;
            listenerResponse.ContentEncoding = Content.ContentEncoding;
            listenerResponse.ContentType = Content.ContentType;
            
            Content.WriteToStreamAsync(listenerResponse.OutputStream);
            listenerResponse.OutputStream.Close();
        }
    }
}