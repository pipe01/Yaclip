using System.Collections.Generic;

namespace LogicCommandLineParser
{
    internal static class Extensions
    {
#nullable disable
        public static T PeekOrDefault<T>(this Queue<T> queue) => queue.Count == 0 ? default : queue.Dequeue();
#nullable enable
    }
}
