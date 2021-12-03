using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Core.Events
{
    public class AsyncEventHandler<T> where T : Delegate
    {
        private readonly ICollection<T> _events;

        public AsyncEventHandler()
        {
            this._events = new HashSet<T>();
        }

        public void Add(T value)
        {
            this._events.Add(value);
        }

        public void Remove(T value)
        {
            this._events.Remove(value);
        }

        public Task InvokeAsync(Func<T, Task> func)
        {
            return Task.WhenAll(this.Call(func));
        }

        private IEnumerable<Task> Call(Func<T, Task> func)
        {
            foreach (var @delegate in this._events)
            {
                yield return func(@delegate);
            }
        }
    }
}
