namespace Cab2Calibre
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    public class Program
    {
        #region Constants

        private const string SOURCE_DIR = ".";
        private const string TARGET_DIR = "_export";

        #endregion

        #region Fields

        private readonly FileSystem _fs;
        private readonly string _targetDir;

        private int lastID = 1;

        #endregion

        #region Constructors and Destructors

        public Program(string targetDir, FileSystem fs)
        {
            _targetDir = targetDir;
            _fs = fs;
        }

        public Program(string targetDir) : this(targetDir, new FileSystem())
        {
        }

        #endregion

        #region Properties

        private static IEnumerable<string> AllTags
        {
            get
            {
                var tags = ConfigurationManager.AppSettings["ExportTags"] ?? string.Empty;
                return tags.Split(',').NotNullOrEmpty().Trimmed().Distinct();
            }
        }

        #endregion

        #region Public Methods and Operators

        public static int Main()
        {
            var source = new Cabinet(SOURCE_DIR);
            var program = new Program(Path.Combine(@"/media/E01C57B51C578586/ebooks", TARGET_DIR));
            //var program = new Program(Path.Combine(@"C:\Users\User\_export", TARGET_DIR));

            program.Export(source, AllTags);

            Console.ReadLine();
            return 0;
        }

        #endregion

        #region Methods

        private void CopyBook(Cabinet source, Book book, string dname)
        {
            string targetFilePath = Path.Combine(dname, book.FileNameInLib);
            string sourceFilePath = book.FullPath(source.HomePath);
            _fs.CopyFile(sourceFilePath, targetFilePath);
        }

        private void Export(Cabinet source, IEnumerable<string> tags)
        {
            ValidateTargetPath(_targetDir);

            List<string> tagList = tags.ToList();
            ProcessCab(source, _targetDir, tagList);
            ProcessChildCabs(source, tagList);
        }

        private void ProcessCab(Cabinet source, string targetDir, IEnumerable<string> tags)
        {
            Console.WriteLine("\nprocessing {0}", source.HomePath);

            IEnumerable<Book> books = source.Books;
            var opfWriter = new OpfWriter();

            Book prev = Book.Null;
            string dname = string.Empty;

            books.ForEach(
                (id, book) =>
                    {
                        id = lastID + id;
                        if (book.ToString() != prev.ToString())
                        {
                            Console.WriteLine(book.ToString());
                            dname = Path.Combine(targetDir, book.DirNameInLib(id));
                            _fs.MakeDir(dname);
                            opfWriter.WriteTo(dname, book, id, tags);
                        }

                        CopyBook(source, book, dname);

                        prev = book;
                    });

            lastID += 100;
        }

        private void ProcessChildCabs(Cabinet source, List<string> tags)
        {
            foreach (Cabinet cabinet in source.Children)
            {
                Export(cabinet, tags.Concat(cabinet.Title.ToAtom()).Distinct());
            }
        }

        private void ValidateTargetPath(string targetPath)
        {
            if (!Directory.Exists(targetPath))
            {
                _fs.MakeDir(targetPath);
            }
            else if (File.Exists(targetPath))
            {
                throw new ArgumentException(string.Format("'{0}' no es un directorio valido", targetPath), "targetPath");
            }
        }

        #endregion
    }
}