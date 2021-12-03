using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static Task ForEach<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (var element in elements)
            {
                action(element);
            }

            return Task.CompletedTask;
        }

        public static async Task ForEach<T>(this IEnumerable<T> sequence, Func<T, Task<bool>> action)
        {
            foreach (T obj in sequence)
            {
                if (await action(obj))
                    break;
            }
        }
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}
