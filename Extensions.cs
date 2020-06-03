using System.Collections.Generic;
using System.Linq;

namespace Yaclip
{
    internal static class Extensions
    {
#nullable disable
        public static T PeekOrDefault<T>(this Queue<T> queue) => queue.Count == 0 ? default : queue.Peek();
#nullable enable

        public static bool ContainsAllItems<T>(this IEnumerable<T> a, IEnumerable<T> b) => !b.Except(a).Any();

        public static bool StartsWith<T>(this T[] a, T[] b)
        {
            if (b.Length > a.Length)
                return false;

            int i = 0;
            bool anyElemEqual = true;

            while (i < a.Length)
            {
                if (i >= b.Length)
                    break;

                anyElemEqual = Equals(a[i], b[i]);

                if (!anyElemEqual)
                    return false;

                i++;
            }

            return anyElemEqual;
        }
    }
}
