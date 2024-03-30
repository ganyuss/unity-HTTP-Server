using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityHttpServer.Controller
{
    /// <summary>
    /// Controller can implement this method to provide HTTP request
    /// consumers dynamically.
    /// </summary>
    /// <seealso cref="IHttpRequestConsumer"/>
    [PublicAPI]
    public interface IDynamicConsumerProvider
    {
        /// <summary>
        /// Returns the list of dynamic HTTP request consumers if the
        /// controller. 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<IHttpRequestConsumer> GetDynamicMethods();
    }
}