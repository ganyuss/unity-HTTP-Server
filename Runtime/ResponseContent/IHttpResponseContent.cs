using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnityHttpServer
{
    public interface IHttpResponseContent
    {
        public abstract long ContentSize { get; }
        public abstract string ContentType { get; }
        
        public virtual Encoding ContentEncoding => Encoding.Default;
        
        public Task WriteToStreamAsync(Stream output);
    }
}