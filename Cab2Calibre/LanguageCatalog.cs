namespace Cab2Calibre
{
    using System.Collections.Generic;
    using System.Linq;

    public static class LanguageCatalog
    {
        #region Static Fields

        private static readonly IDictionary<string, string> Languages = new Dictionary<string, string>
        {
            { "english",    "eng" },
            { "en",         "eng" },
            { "eng",        "eng" },
            { "ingles",     "eng" },
            { "inglés",     "eng" },
    
            { "fr",         "fra" },
            { "fra",        "fra" },
            { "frances",    "fra" },
            { "francés",    "fra" },
            { "french",     "fra" },
	
            { "aleman",     "ger" },
            { "alemán",     "ger" },
            { "de",         "ger" },
            { "ger",        "ger" },
            { "german",     "ger" },
	
            { "ja",         "jpn" },
            { "jap",        "jpn" },
            { "japanese",   "jpn" },
            { "japones",    "jpn" },
            { "japonés",    "jpn" },
            { "jpn",        "jpn" },

            { "pl",         "pol" },
            { "pol",        "pol" },
            { "polaco",     "pol" },
            { "polish",     "pol" },
	
            { "por",        "por" },
            { "portugues",  "por" },
            { "portugués",  "por" },
            { "portuguese", "por" },
            { "pt",         "por" },
	
            { "ru",         "rus" },
            { "rus",        "rus" },
            { "ruso",       "rus" },
            { "russian",    "rus" },
	
            { "es",         "spa" },
            { "espanol",    "spa" },
            { "español",    "spa" },
            { "spa",        "spa" },
            { "spanish",    "spa" },
	
            { "chi",        "zho" },
            { "chinese",    "zho" },
            { "chino",      "zho" },
            { "zh",         "zho" },
            { "zho",        "zho" },
        };

        #endregion

        #region Public Properties

        public static IEnumerable<KeyValuePair<string, string>> Items => Languages.AsEnumerable();

        #endregion

        #region Public Methods and Operators

        public static string GetIsoCodeFor(string name, string orDefault)
        {
            return Languages[Languages.ContainsKey(name) ? name : orDefault];
        }

        #endregion
    }
}