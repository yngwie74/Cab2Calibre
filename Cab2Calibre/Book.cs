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

        private readonly string[] data;

        #endregion

        #region Constructors and Destructors

        public Book(string line)
        {
            var parts = line.SplitBy('\t').Trimmed().ToArray();
            FileName = parts[0];
            this.data = parts.Skip(1).ToArray();
        }

        #endregion

        #region Public Properties

        public IEnumerable<string> Authors => SortableAuthors.Select(ReverseNameOrder);

        public string Date => int.TryParse(this.data[4], out var year) ? $"{year}-01-01T00:00:00-06:00" : string.Empty;

        public int Edition => JoinTitle(FindMainTitle(), SubTitle).GetEditionNumber();

        public string FileName { get; }

        public string FileNameInLib
        {
            get
            {
                var ext = Path.GetExtension(FileName);
                var result = $"{MainTitle} - {MainAuthor}".ToFsName(MaxFsNameLength);
                return result + ext;
            }
        }

        public string FullTitle => JoinTitle(MainTitle, SubTitle).FormatEdition(Edition, Language);

        public string Language
        {
            get
            {
                var language = (this.data.Length >= 6)
                                   ? this.data[5].ToLower()
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
                var result = FindMainTitle().RemoveEdition();
                return TitleSufixes.Where(s => result.ToLower().EndsWith(", " + s))
                    .Aggregate(
                        result,
                        (title, suffix) => $"{suffix.Capitalize()} {GetTitleText(title, suffix)}");
            }
        }

        public string Publisher => this.data[3];

        public IEnumerable<string> SortableAuthors => this.data[2].SplitBy('|').NotNullOrEmpty().Select(StripRole);

        public string SubTitle
        {
            get
            {
                var result = this.data[1];
                if (string.IsNullOrEmpty(this.data[0]))
                {
                    result = string.Empty;
                }

                return result;
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool BookExists(string cabPath)
        {
            return File.Exists(FullPath(cabPath));
        }

        public string DirNameInLib(int id)
        {
            var parent = MainAuthor.ToFsName(MaxFsNameLength);
            var sid = $" ({id})";
            var title = FullTitle.Replace(':', '_').ToFsName(MaxFsNameLength - sid.Length);
            var child = $"{title}{sid}";
            return Path.Combine(parent, child);
        }

        public string FullPath(string cabPath)
        {
            var fullPath = Path.Combine(cabPath, FileName);
            return fullPath;
        }

        public override string ToString()
        {
            return $"{FullTitle} by {MainAuthor}";
        }

        #endregion

        #region Methods

        private static string GetTitleText(string title, string suffix)
        {
            return title.Substring(0, title.Length - suffix.Length - 2);
        }

        private static string JoinTitle(string mainTitle, string subtitle)
        {
            return !string.IsNullOrEmpty(subtitle) ? $"{mainTitle}: {subtitle}" : mainTitle;
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
            var result = this.data[0];
            if (string.IsNullOrEmpty(result))
            {
                result = this.data[1];
            }

            return result;
        }

        #endregion
    }
}