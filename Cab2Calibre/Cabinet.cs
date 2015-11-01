namespace Cab2Calibre
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Cabinet
    {
        #region Constants

        private const string INDEX_FILENAME = "articles.idx";

        #endregion

        #region Fields

        private readonly string _homePath;

        #endregion

        #region Constructors and Destructors

        public Cabinet(string path)
        {
            if (!IsCabinet(path))
            {
                var message = string.Format("'{0}' no es un directorio CAB", path);
                throw new ArgumentException(message, "path");
            }

            _homePath = Path.GetFullPath(path);
        }

        #endregion

        #region Public Properties

        public IEnumerable<Book> Books
        {
            get
            {
                return ReadIndex().Values.Where(b => b.BookExists(HomePath)).OrderBy(b => b.ToString());
            }
        }

        public string HomePath
        {
            get { return _homePath; }
        }

        public string Title
        {
            get { return Path.GetFileName(HomePath); }
        }

        #endregion

        #region Public Methods and Operators

        public static bool IsCabinet(string path)
        {
            return File.Exists(IndexFilePath(path));
        }

        public IEnumerable<Cabinet> Children
        {
            get
            {
                return SubdirsOf(HomePath).Where(IsCabinet).Select(dir => new Cabinet(dir));
            }
        }

        #endregion

        #region Methods

        private static string IndexFilePath(string path)
        {
            return Path.Combine(path, INDEX_FILENAME);
        }

        private static IEnumerable<string> SubdirsOf(string sourcePath)
        {
			return Directory.EnumerateDirectories(sourcePath, "*", SearchOption.TopDirectoryOnly);
        }

        private IDictionary<string, Book> ReadIndex()
        {
            Func<string, Book> parse = line => new Book(line);

            var indexFilePath = IndexFilePath(HomePath);
            using (var f = new StreamReader(indexFilePath, Encoding.GetEncoding("Windows-1250")))
            {
                return f.ReadLines().Select(parse).ToDictionary(b => b.FileName, b => b);
            }
        }

        #endregion
    }
}