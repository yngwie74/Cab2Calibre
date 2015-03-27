namespace Cab2Calibre
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal static class EditionTranslator
    {
        #region Constants

        private const RegexOptions RE_OPTIONS = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        private const string RE_PATTERN = @"^(?<prev>.*), *(?<number>\d{1,2})[Ee]\b(?<after>.*)$";
        private static readonly Regex Matcher = new Regex(RE_PATTERN, RE_OPTIONS);

        private static readonly string[] EnglishSufixes = { "th", "st", "nd", "rd" };

        private static readonly IDictionary<string, Func<string, string>> Translators =
            new Dictionary<string, Func<string, string>>
                {
                    {"eng", English},
                    {"spa", Spanish},
                };

        #endregion

        #region Public Methods and Operators

        public static string GetEdition(string title, string language)
        {
            var translate = Translators.ContainsKey(language) ? Translators[language] : Default;
            return translate(title);
        }

        public static int GetNumber(string title)
        {
            var match = Matcher.Match(title);
            if (match.Success)
            {
                return Int32.Parse(match.Groups["number"].Value);
            }
            return 1;
        }

        public static string RemoveEdition(string title)
        {
            return Matcher.IsMatch(title) ? Matcher.Replace(title, "$1$3") : title;
        }

        #endregion

        #region Methods

        private static string Default(string title)
        {
            return string.Format("{0}E", GetNumber(title));
        }

        private static string English(string title)
        {
            var n = GetNumber(title);
            return String.Format("{0}{1} Edition", n, GetEnglishSufix(n));
        }

        private static string GetEnglishSufix(int n)
        {
            return EnglishSufixes[n / 10 % 10 == 1 || n % 10 > 3 ? 0 : n % 10];
        }

        private static string Spanish(string title)
        {
            return String.Format("{0}a Edición", GetNumber(title));
        }

        #endregion
    }
}