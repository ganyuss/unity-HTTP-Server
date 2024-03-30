using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityHttpServer.Routing;

namespace UnityHttpServer.Controller
{
    internal class HttpControllerWrapper : IHttpController
    {
        [CanBeNull] 
        private readonly ILogger Logger;
        
        private readonly object Controller;
        private readonly IReadOnlyCollection<IControllerMethod> Methods;
        
        public HttpControllerWrapper([NotNull] object controller, [CanBeNull] ILogger logger)
        {
            Logger = logger;
            
            Controller = controller;
            Methods = GetAllMethods(Controller);
            
            
            Logger?.Log($"Created {Methods.Count} routes for controller {Controller.GetType()}");
        }

        public bool TryConsume(HttpRequest request, out HttpResponse response)
        {
            foreach (var method in Methods)
            {
                if (!method.Match(request)) 
                    continue;
                
                response = method.Consume(request);
                return true;
            }

            response = default;
            return false;
        }

        private List<IControllerMethod> GetAllMethods(object controller)
        {
            List<IControllerMethod> output = new List<IControllerMethod>();
            
            output.AddRange(GetAllMethodsFromDynamicProvider(controller));
            output.AddRange(GetAllMethodsFromAttributes(controller));

            return output;
        }

        private IEnumerable<IControllerMethod> GetAllMethodsFromDynamicProvider(object controller)
        {
            if (controller is IDynamicMethodProvider dynamicMethodProvider)
                return dynamicMethodProvider.GetDynamicMethods();
            
            return Enumerable.Empty<IControllerMethod>();
        }

        private List<IControllerMethod> GetAllMethodsFromAttributes(object controller)
        {
            List<IControllerMethod> output = new List<IControllerMethod>();
            var methodInfos = controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var methodInfo in methodInfos)
            {
                var routeAttributes = methodInfo.GetCustomAttributes()
                    .Where(attr => attr is RouteAttribute)
                    .Cast<RouteAttribute>();
                
                foreach (RouteAttribute attribute in routeAttributes)
                {
                    if (ReflectionControllerMethod.TryMake(attribute, controller, methodInfo, out var controllerMethod))
                    {
                        output.Add(controllerMethod);
                    }
                    else
                    {
                        Logger?.Log(LogType.Error, $"Cannot create route from attribute {attribute.GetType()} on method {Controller.GetType()}.{methodInfo.Name}");
                    }
                }
            }

            return output;
        }
    }
}