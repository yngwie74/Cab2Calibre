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

        #endregion

        #region Fields

        private static string[] allTags;

        private readonly FileSystem fs;
        private readonly string targetDir;

        private int lastId = 1;

        #endregion

        #region Constructors and Destructors

        public Program(string targetDir, FileSystem fs)
        {
	        this.targetDir = targetDir;
	        this.fs = fs;
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
                if (allTags == null)
                {
                    allTags = GetConfigNamed("ExportTags", orElse: string.Empty).Split(',');
                }

                return allTags.Trimmed().NotNullOrEmpty().Distinct();
            }
        }

        private static string TargetDir => Path.GetFullPath(GetConfigNamed("TargetDir", orElse: Path.Combine(".", "_export")));

        #endregion

        #region Public Methods and Operators

        public static int Main()
        {
            var source = new Cabinet(SOURCE_DIR);
            var program = new Program(TargetDir);

            program.Export(source, AllTags);

            Console.ReadLine();
            return 0;
        }

        #endregion

        #region Methods

        private static string GetConfigNamed(string name, string orElse)
        {
            return ConfigurationManager.AppSettings[name] ?? orElse;
        }

        private void CopyBook(Cabinet source, Book book, string dirName)
        {
            var targetFilePath = Path.Combine(dirName, book.FileNameInLib);
            var sourceFilePath = book.FullPath(source.HomePath);
            this.fs.CopyFile(sourceFilePath, targetFilePath);
        }

        private void Export(Cabinet source, IEnumerable<string> tags)
        {
            ValidateTargetPath(this.targetDir);

            var tagList = tags.ToList();
            ProcessCab(source, this.targetDir, tagList);
            ProcessChildCabs(source, tagList);
        }

        private void ProcessCab(Cabinet source, string currentDir, IEnumerable<string> tags)
        {
            Console.WriteLine("\nprocessing {0}", source.HomePath);

            var books = source.Books;
            var opfWriter = new OpfWriter();

            var prev = Book.Null;
            var dirName = string.Empty;

            books.ForEach(
                (id, book) =>
                    {
                        id = this.lastId + id;
                        if (book.ToString() != prev.ToString())
                        {
                            Console.WriteLine(book.ToString());
                            dirName = Path.Combine(currentDir, book.DirNameInLib(id));
                            this.fs.MakeDir(dirName);
                            opfWriter.WriteTo(dirName, book, id, tags);
                        }

                        CopyBook(source, book, dirName);

                        prev = book;
                    });

            this.lastId += 100;
        }

        private void ProcessChildCabs(Cabinet source, List<string> tags)
        {
            foreach (var cabinet in source.Children)
            {
                Export(cabinet, tags.Concat(cabinet.Title.ToAtom()).Distinct());
            }
        }

        private void ValidateTargetPath(string targetPath)
        {
            if (!Directory.Exists(targetPath))
            {
	            this.fs.MakeDir(targetPath);
            }
            else if (File.Exists(targetPath))
            {
                throw new ArgumentException($"'{targetPath}' no es un directorio válido", nameof(targetPath));
            }
        }

        #endregion
    }
}