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
            KeepAlive = listenerRequest.KeepAlive;
            ProtocolVersion = listenerRequest.ProtocolVersion;
            AcceptedTypes = listenerRequest.AcceptTypes;
        }
        
        public HttpMethod Method { get; }
        public Uri Uri { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        
        private Stream ContentStream { get; }
        private System.Text.Encoding ContentEncoding { get; }
        
        public Cookie[] Cookies { get; }
        public bool KeepAlive { get; }
        public Version ProtocolVersion { get; }
        [CanBeNull] public string[] AcceptedTypes { get; }

        
        public async Task<string> GetStringContentAsync()
        {
            StreamReader reader = new StreamReader(ContentStream, ContentEncoding);
            return await reader.ReadToEndAsync();
        }

        public string GetStringContent()
        {
            StreamReader reader = new StreamReader(ContentStream, ContentEncoding);
            return reader.ReadToEnd();
        }
        
        /*
        Debug.LogFormat("Local end point: {0}", request.LocalEndPoint);
        Debug.LogFormat("Remote end point: {0}", request.RemoteEndPoint);
        Debug.LogFormat("Is local? {0}", request.IsLocal);
        Debug.LogFormat("HTTP method: {0}", request.HttpMethod);
        Debug.LogFormat("Protocol version: {0}", request.ProtocolVersion);
        Debug.LogFormat("Is authenticated: {0}", request.IsAuthenticated);
        Debug.LogFormat("Is secure: {0}", request.IsSecureConnection);
        */
    }
}