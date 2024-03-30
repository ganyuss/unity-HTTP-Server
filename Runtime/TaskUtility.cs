using System.Threading.Tasks;
using UnityEngine;

namespace UnityHttpServer
{
    internal static class TaskUtility
    {
        public static Task ListenForErrors(this Task task)
        {
            return task.ContinueWith(t =>
            {
                if (t.Exception != null)
                    Debug.LogException(t.Exception);

            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}