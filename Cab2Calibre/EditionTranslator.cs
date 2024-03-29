﻿namespace Cab2Calibre
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Translator = System.Func<int, string>;

    internal static class EditionTranslator
    {
        #region Constants

        private const RegexOptions RE_OPTIONS = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        private const string RE_PATTERN = @"^(?<prev>.*), *(?<number>\d{1,2})[Ee]\b(?<after>.*)$";

        private static readonly string[] EnglishSuffixes = { "th", "st", "nd", "rd" };

        private static readonly Regex Matcher = new Regex(RE_PATTERN, RE_OPTIONS);

        private static readonly IDictionary<string, Translator> Translators = 
            new Dictionary<string, Translator>
            {
                { "eng", English },
                { "spa", Spanish },
            };

        #endregion

        #region Public Methods and Operators

        public static string FormatEdition(this string title, int editionNumber, string language)
        {
            title = title.RemoveEdition();
            return editionNumber > 1 ? $"{title}, {GetEdition(editionNumber, language)}" : title;
        }

        public static int GetEditionNumber(this string title)
        {
            var match = Matcher.Match(title);
            return match.Success ? Int32.Parse(match.Groups["number"].Value) : 1;
        }

        public static string RemoveEdition(this string title)
        {
            return Matcher.IsMatch(title) ? Matcher.Replace(title, "$1$3") : title;
        }

        #endregion

        #region Methods

        private static string Default(int editionNumber)
        {
            return $"{editionNumber}E";
        }

        private static string English(int editionNumber)
        {
            return $"{editionNumber}{GetEnglishSuffix(editionNumber)} Edition";
        }

        internal static string GetEdition(int editionNumber, string language)
        {
            var translate = Translators.ContainsKey(language) ? Translators[language] : Default;
            return translate(editionNumber);
        }

        private static string GetEnglishSuffix(int n)
        {
            return EnglishSuffixes[n / 10 % 10 == 1 || n % 10 > 3 ? 0 : n % 10];
        }

        private static string Spanish(int editionNumber)
        {
            return $"{editionNumber}a Edición";
        }

        #endregion
    }
}
