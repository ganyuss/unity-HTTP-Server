using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnityHttpServer.Controller
{
    public class DynamicAwaitable
    {
        private readonly object Awaitable;
        
        private readonly MethodInfo GetAwaiterMethod;

        public DynamicAwaitable(object awaitable)
        {
            Awaitable = awaitable;

            GetAwaiterMethod = Awaitable.GetType().GetMethod(nameof(GetAwaiter));
        }

        public DynamicAwaiter GetAwaiter()
        {
            var awaiter = GetAwaiterMethod.Invoke(Awaitable, Array.Empty<object>());
            return new DynamicAwaiter(awaiter);
        }

        public Type ReturnType => GetAwaiter().ReturnType;
    }

    public class DynamicAwaiter : INotifyCompletion
    {
        private readonly object Awaiter;

        private readonly MethodInfo IsCompletedGetter;
        private readonly MethodInfo OnCompletedMethod;
        private readonly MethodInfo GetResultMethod;
        
        public DynamicAwaiter(object awaiter)
        {
            Awaiter = awaiter;

            var awaiterType = Awaiter.GetType();
            IsCompletedGetter = awaiterType.GetProperty(nameof(IsCompleted))!.GetMethod;
            OnCompletedMethod = awaiterType.GetMethod(nameof(OnCompleted));
            GetResultMethod = awaiterType.GetMethod(nameof(GetResult));
        }

        public Type ReturnType => GetResultMethod.ReturnType;

        public bool IsCompleted => (bool) IsCompletedGetter.Invoke(Awaiter, null);

        public void OnCompleted(Action continuation)
        {
            OnCompletedMethod.Invoke(Awaiter, new object[] { continuation });
        }

        public object GetResult()
        {
            return GetResultMethod.Invoke(Awaiter, null);
        }
    }
}