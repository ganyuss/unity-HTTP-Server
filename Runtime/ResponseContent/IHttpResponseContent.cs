using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnityHttpServer
{
    /// <summary>
    /// Represents the content of an HTTP response, to be sent asynchronously.
    /// </summary>
    public interface IHttpResponseContent
    {
        /// <summary>
        /// The size of content in bytes. 
        /// </summary>
        public long ContentSize { get; }
        /// <summary>
        /// The mime type of content.
        /// </summary>
        /// <seealso cref="System.Net.Mime.MediaTypeNames"/>
        public string ContentType { get; }
        
        /// <summary>
        /// The optional encoding of the content. 
        /// </summary>
        public Encoding ContentEncoding => Encoding.Default;
        
        /// <summary>
        /// Implement this method to write content
        /// to the response stream.
        /// </summary>
        /// <param name="output">The stream to write to.</param>
        /// <returns></returns>
        /// <remarks><b>DO NOT</b> close the stream, only write to it.</remarks>
        public Task WriteToStreamAsync(Stream output);
    }
}