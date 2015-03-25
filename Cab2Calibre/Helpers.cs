namespace Cab2Calibre
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class Helpers
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            var index = 0;
            foreach (var item in source)
            {
                action(index++, item);
            }
        }

        public static IEnumerable<string> ReadLines(this TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}