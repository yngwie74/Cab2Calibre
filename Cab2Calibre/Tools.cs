namespace Cab2Calibre
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal static class Tools
    {
        public static string Capitalize(this string arg)
        {
            if (String.IsNullOrEmpty(arg))
            {
                return arg;
            }

            if (arg.Length == 1)
            {
                return arg.ToUpper();
            }

            return Char.ToUpper(arg[0]) + arg.Substring(1).ToLower();
        }

        public static string FilterPrintableAscii(this string arg)
        {
            return arg
                .Normalize(NormalizationForm.FormKD)
                .Where(IsPrintableAscii)
                .GatherString()
                .Normalize(NormalizationForm.FormC);
        }

        public static string GatherString(this IEnumerable<char> chars)
        {
            return String.Concat(chars);
        }

        public static bool IsPrintableAscii(this char arg)
        {
            return (arg <= 127) && !Char.IsControl(arg);
        }

        public static string JoinUsing(this IEnumerable<string> strings, string separator)
        {
            return String.Join(separator, strings);
        }

        public static IEnumerable<string> NotNullOrEmpty(this IEnumerable<string> strings)
        {
            return strings.Where(s => !String.IsNullOrEmpty(s));
        }

        public static string[] SplitBy(this string arg, char separator)
        {
            return arg.Split(new[] { separator }, StringSplitOptions.None);
        }

        public static string[] SplitBy(this string arg, char separator, int maxParts)
        {
            return arg.Split(new[] { separator }, maxParts);
        }

        public static IEnumerable<T> ToAtom<T>(this T name)
        {
            return Enumerable.Repeat(name, 1);
        }

        public static string ToFsName(this string name, int maxLength)
        {
            return
                name.FilterPrintableAscii()
                    .Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)
                    .JoinUsing("_")
                    .TruncateAt(maxLength)
                    .TrimEnd();
        }

        public static IEnumerable<string> Trimmed(this IEnumerable<string> args)
        {
            return args.Select(s => s.Trim());
        }

        private static string TruncateAt(this string arg, int lenght)
        {
            return arg.Length > lenght ? arg.Substring(0, lenght) : arg;
        }
    }
}