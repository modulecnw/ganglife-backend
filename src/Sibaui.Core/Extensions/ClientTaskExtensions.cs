using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Core.Extensions
{
    public static class ClientTaskExtensions
    {
        public static System.Threading.Tasks.Task<T> RunReturn<T>(this GTANetworkMethods.Task task, System.Func<T> func)
        {
            var taskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            task.Run(() =>
            {
                var result = func();
                taskCompletionSource.SetResult(result);
            });
            return taskCompletionSource.Task;
        }
    }
}
