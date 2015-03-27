namespace Cab2Calibre
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Filter = System.Func<string, System.Collections.Generic.IEnumerable<string>>;

    public class Book
    {
        #region Constants

        private const string English = "english";

        private const string DefaultLanguage = English;

        private const int MaxFsNameLength = 50;

        #endregion

        #region Static Fields

        public static readonly Book Null = new Book("\t\t\t\t\t\t\t");

        private static readonly string[] TitleSufixes = { "a", "an", "the", "el", "la", "las", "los" };

        #endregion

        #region Fields

        private readonly string[] _data;

        private readonly string _fileName;

        #endregion

        public bool BookExists(string cabPath)
        {
            //Console.WriteLine(full_path);
            return File.Exists(FullPath(cabPath));
        }

        public string FullPath(string cabPath)
        {
            var fullPath = Path.Combine(cabPath, FileName);
            return fullPath;
        }

        #region Constructors and Destructors

        public Book(string line)
        {
            var parts = line.SplitBy('\t').Trimmed().ToArray();
            _fileName = parts[0];
            _data = parts.Skip(1).ToArray();
        }

        #endregion

        #region Public Properties

        public IEnumerable<string> Authors
        {
            get
            {
                return SortableAuthors.Select(ReverseNameOrder);
            }
        }

        public string Date
        {
            get
            {
                int year;
                return int.TryParse(_data[4], out year) ? string.Format("{0}-01-01T00:00:00-06:00", year) : string.Empty;
            }
        }

        public string FileNameInLib
        {
            get
            {
                var ext = Path.GetExtension(FileName);
                var result = string.Format("{0} - {1}", MainTitle, MainAuthor).ToFsName(MaxFsNameLength);
                return result + ext;
            }
        }

        public string FullTitle
        {
            get
            {
                var subtitle = SubTitle;
                return string.IsNullOrEmpty(subtitle) 
                    ? MainTitle 
                    : string.Format("{0}: {1}", MainTitle, subtitle);
            }
        }

        public string Language
        {
            get
            {
                var language = (_data.Length >= 6)
                                   ? _data[5].ToLower()
                                   : LanguageCatalog.GetIsoCodeFor(DefaultLanguage, DefaultLanguage);

                return LanguageCatalog.GetIsoCodeFor(language, DefaultLanguage);
            }
        }

        public string MainAuthor
        {
            get
            {
                const string Default = "Unknown";
                var result = Authors.FirstOrDefault();
                return string.IsNullOrEmpty(result) ? Default : result;
            }
        }

        public string MainTitle
        {
            get
            {
                var result = FindMainTitle();
                return TitleSufixes.Where(s => result.ToLower().EndsWith(", " + s))
                    .Aggregate(
                        result,
                        (title, suffix) => string.Format("{0} {1}", suffix.Capitalize(), GetTitleText(title, suffix)));
            }
        }

        public string Publisher
        {
            get { return _data[3]; }
        }

        public IEnumerable<string> SortableAuthors
        {
            get
            {
                return _data[2].SplitBy('|').NotNullOrEmpty().Select(StripRole);
            }
        }

        public string SubTitle
        {
            get
            {
                var result = _data[1];
                if (string.IsNullOrEmpty(_data[0]))
                {
                    result = string.Empty;
                }

                return result;
            }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        #endregion

        #region Public Methods and Operators

        public string DirNameInLib(int id)
        {
            var parent = MainAuthor.ToFsName(MaxFsNameLength);
            var sid = string.Format(" ({0})", id);
			var title = FullTitle.Replace(':', '_').ToFsName(MaxFsNameLength - sid.Length);
            var child = string.Format("{0}{1}", title, sid);
            return Path.Combine(parent, child);
        }

        public override string ToString()
        {
            return string.Format("{0} by {1}", FullTitle, MainAuthor);
        }

        #endregion

        #region Methods

        private static string GetTitleText(string title, string suffix)
        {
            return title.Substring(0, title.Length - suffix.Length - 2);
        }

        private static string ReverseNameOrder(string name)
        {
            Filter splitTrimReverse = str => str.SplitBy(',', 2).Trimmed().Reverse();
            var filter = name.Contains(',') ? splitTrimReverse : Tools.ToAtom;

            return filter(name).JoinUsing(" ");
        }

        private static string StripRole(string author)
        {
            return author.SplitBy('(', 2).First().Trim();
        }

        private string FindMainTitle()
        {
            var result = _data[0];
            if (string.IsNullOrEmpty(result))
            {
                result = _data[1];
            }

            return result;
        }

        #endregion
    }
}