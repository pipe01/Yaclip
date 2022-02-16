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

        public static bool StartsWith<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b)
        {
            if (b.Count > a.Count)
                return false;

            int len = Math.Min(a.Count, b.Count);
            int i;

            for (i = 0; i < len; i++)
            {
                if (!Equals(a[i], b[i]))
                    return false;
            }

            return i >= b.Count - 1;
        }

        public static bool IsListType(this Type type, out Type itemType)
        {
            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();

                if (genericDef == typeof(IList<>) || genericDef == typeof(IReadOnlyList<>)) {
                    itemType = type.GetGenericArguments()[0];
                    return true;
                }
            }

            itemType = null;
            return false;
        }
    }
}
