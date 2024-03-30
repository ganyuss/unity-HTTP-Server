using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityHttpServer.Controller
{
    [PublicAPI]
    public interface IDynamicMethodProvider
    {
        public IEnumerable<IControllerMethod> GetDynamicMethods();
    }
}