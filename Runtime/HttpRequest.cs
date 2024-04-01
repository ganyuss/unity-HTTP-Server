using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace UnityHttpServer
{
    /// <summary>
    /// Represents the input of an HTTP request.
    /// </summary>
    [PublicAPI]
    public class HttpRequest
    {
        internal HttpRequest(HttpListenerRequest listenerRequest)
        {
            Method = new HttpMethod(listenerRequest.HttpMethod);
            Uri = listenerRequest.Url;
            
            Headers = listenerRequest.Headers.AllKeys.ToDictionary(key => key,
                key => listenerRequest.Headers[key]);

            ContentStream = listenerRequest.InputStream;

            Cookies = listenerRequest.Cookies.Cast<Cookie>().ToArray();
            ProtocolVersion = listenerRequest.ProtocolVersion;
            AcceptTypes = listenerRequest.AcceptTypes;
            
            if (listenerRequest.Headers.AllKeys.Contains("Accept-Encoding"))
                AcceptEncoding = listenerRequest.Headers["Accept-Encoding"]
                    .Split(",")
                    .Select(s => s.Trim())
                    .ToArray();
        }
        
        /// <summary>
        /// The input HTTP method
        /// </summary>
        public HttpMethod Method { get; }
        /// <summary>
        /// The request URI
        /// </summary>
        public Uri Uri { get; }
        /// <summary>
        /// The request headers
        /// </summary>
        public IReadOnlyDictionary<string, string> Headers { get; }
        
        /// <summary>
        /// The request content
        /// </summary>
        private Stream ContentStream { get; }
        /// <summary>
        /// The request content encoding
        /// </summary>
        private System.Text.Encoding ContentEncoding { get; }
        
        /// <summary>
        /// The request cookies
        /// </summary>
        public Cookie[] Cookies { get; }
        /// <summary>
        /// The request protocol version
        /// </summary>
        public Version ProtocolVersion { get; }
        
        /// <summary>
        /// The accepted mime types for the response
        /// </summary>
        [CanBeNull]
        public string[] AcceptTypes { get; }
        
        /// <summary>
        /// The accepted encodings for the response
        /// </summary>
        [CanBeNull]
        public string[] AcceptEncoding { get; }

        /// <summary>
        /// Returns the content of the request as a string
        /// </summary>
        /// <seealso cref="ContentStream"/>
        public async Task<string> GetStringContentAsync()
        {
            StreamReader reader = new StreamReader(ContentStream, ContentEncoding);
            return await reader.ReadToEndAsync();
        }

        /// <inheritdoc cref="GetStringContentAsync"/>
        public string GetStringContent()
        {
            StreamReader reader = new StreamReader(ContentStream, ContentEncoding);
            return reader.ReadToEnd();
        }
    }
}