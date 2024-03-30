using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityHttpServer.Routing;

namespace UnityHttpServer.Controller
{
    public class ReflectionControllerMethod : IControllerMethod
    {
        private readonly RouteAttribute RouteAttribute;
        private readonly MethodInfo MethodInfo;
        private readonly object Controller;

        public static bool TryMake(RouteAttribute routeAttribute, object controller, MethodInfo methodInfo, out ReflectionControllerMethod controllerMethod)
        {
            controllerMethod = null;

            if (methodInfo.ReturnType != typeof(void)
                && ! typeof(HttpResponse).IsAssignableFrom(methodInfo.ReturnType))
                return false;

            if (methodInfo.GetParameters().Any(parameter => parameter.IsIn))
                return false;

            controllerMethod = new ReflectionControllerMethod(routeAttribute, controller, methodInfo);
            return true;
        }

        private ReflectionControllerMethod(RouteAttribute routeAttribute, object controller, MethodInfo methodInfo)
        {
            RouteAttribute = routeAttribute;
            MethodInfo = methodInfo;
            Controller = controller;
        }


        public bool Match(HttpRequest request)
        {
            return RouteAttribute.MatchRequest(request);
        }

        public HttpResponse Consume(HttpRequest request)
        {
            object[] parameters = CreateReflectionParameters(MethodInfo, request);
            object output;
            try
            {
                output = MethodInfo.Invoke(Controller, parameters);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return HttpStatusCode.InternalServerError;
            }
            
            bool isMethodVoid = MethodInfo.ReturnType == typeof(void);
            if (isMethodVoid)
                return HttpStatusCode.OK;

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