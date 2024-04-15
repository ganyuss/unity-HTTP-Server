using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityHttpServer.Routing;

namespace UnityHttpServer.Controller
{
    internal class ReflectionHttpRequestConsumer : IHttpRequestConsumer
    {
        private readonly ConsumerAttribute _consumerAttribute;
        private readonly MethodInfo MethodInfo;
        private readonly object Controller;

        public static bool TryMake(ConsumerAttribute consumerAttribute, object controller, MethodInfo methodInfo, out ReflectionHttpRequestConsumer httpRequestConsumer)
        {
            httpRequestConsumer = null;

            if (methodInfo.ReturnType != typeof(void)
                && ! typeof(HttpResponse).IsAssignableFrom(methodInfo.ReturnType))
                return false;

            if (methodInfo.GetParameters().Any(parameter => parameter.IsIn))
                return false;

            httpRequestConsumer = new ReflectionHttpRequestConsumer(consumerAttribute, controller, methodInfo);
            return true;
        }

        private ReflectionHttpRequestConsumer(ConsumerAttribute consumerAttribute, object controller, MethodInfo methodInfo)
        {
            _consumerAttribute = consumerAttribute;
            MethodInfo = methodInfo;
            Controller = controller;
        }


        public bool Match(HttpRequest request)
        {
            return _consumerAttribute.MatchRequest(request);
        }

        public async Task<HttpResponse> ConsumeAsync(HttpRequest request)
        {
            object[] parameters = CreateReflectionParameters(MethodInfo, request);

            var output = MethodInfo.Invoke(Controller, parameters);

            if (MethodInfo.ReturnType == typeof(void))
                return HttpStatusCode.OK;

            // If return value is awaitable, we await
            if (MethodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null)
            {
                // Cannot use await (dynamic) here, to be compatible with IL2CPP
                var dynamicAwaitable = new DynamicAwaitable(output);
                output = await dynamicAwaitable;
                
                if (dynamicAwaitable.ReturnType == typeof(void))
                    return HttpStatusCode.OK;
            }

            return (HttpResponse)output;
        }

        private object[] CreateReflectionParameters(MethodInfo methodInfo, HttpRequest request)
        {
            var parameterInfos = methodInfo.GetParameters();
            
            if (parameterInfos.Length == 0)
                return null;
            
            List<object> output = new List<object>();

            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.ParameterType == typeof(HttpRequest))
                    output.Add(request);
                else 
                    output.Add(GetDefault(parameterInfo.ParameterType));
            }

            return output.ToArray();
        }
        
        private static object GetDefault(Type type)
        {
            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}