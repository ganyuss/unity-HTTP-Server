using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityHttpServer.Routing;

namespace UnityHttpServer.Controller
{
    /// <summary>
    /// This class wraps a given controller, to expose the different
    /// <see cref="IHttpRequestConsumer"/> implemented by it.
    /// </summary>
    internal class HttpControllerWrapperWrapper : IHttpControllerWrapper
    {
        [CanBeNull] 
        private readonly ILogger Logger;
        
        private readonly object Controller;
        private readonly IReadOnlyCollection<IHttpRequestConsumer> StaticConsumers;
        
        public HttpControllerWrapperWrapper([NotNull] object controller, [CanBeNull] ILogger logger)
        {
            Logger = logger;
            
            Controller = controller;
            StaticConsumers = GetStaticConsumers(Controller);
            
            
            Logger?.Log($"Created {StaticConsumers.Count} static routes for controller {Controller.GetType()}");
        }

        [ContractAnnotation("=>true; =>false, response: null")]
        public bool TryConsume([NotNull] HttpRequest request, out HttpResponse response)
        {
            var allMethods = StaticConsumers.Concat(GetDynamicConsumers());
            foreach (var method in allMethods)
            {
                if (!method.Match(request))
                    continue;
            
                response = method.Consume(request);
                return true;
            }

            response = default;
            return false;
        }

        private IEnumerable<IHttpRequestConsumer> GetDynamicConsumers()
        {
            return GetConsumersFromDynamicProvider(Controller);
        }

        [Pure]
        [NotNull]
        private List<IHttpRequestConsumer> GetStaticConsumers([NotNull] object controller)
        {
            List<IHttpRequestConsumer> output = new List<IHttpRequestConsumer>();
            
            output.AddRange(GetConsumersFromAttributes(controller));

            return output;
        }

        [Pure]
        [NotNull]
        private IEnumerable<IHttpRequestConsumer> GetConsumersFromDynamicProvider([NotNull] object controller)
        {
            if (controller is IDynamicConsumerProvider dynamicMethodProvider)
                return dynamicMethodProvider.GetDynamicMethods();
            
            return Enumerable.Empty<IHttpRequestConsumer>();
        }

        [Pure]
        [NotNull]
        private List<IHttpRequestConsumer> GetConsumersFromAttributes([NotNull] object controller)
        {
            List<IHttpRequestConsumer> output = new List<IHttpRequestConsumer>();
            var methodInfos = controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var methodInfo in methodInfos)
            {
                var routeAttributes = methodInfo.GetCustomAttributes()
                    .Where(attr => attr is ConsumerAttribute)
                    .Cast<ConsumerAttribute>();
                
                foreach (ConsumerAttribute attribute in routeAttributes)
                {
                    if (ReflectionHttpRequestConsumer.TryMake(attribute, controller, methodInfo, out var controllerMethod))
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