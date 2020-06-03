using System;
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

            int len = Math.Min(a.Length, b.Length);
            int i;

            for (i = 0; i < len; i++)
            {
                if (!Equals(a[i], b[i]))
                    return false;
            }

            return i >= b.Length - 1;
        }
    }
}
