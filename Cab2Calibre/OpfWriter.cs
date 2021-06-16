namespace Cab2Calibre
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class OpfWriter
    {
        #region Constants

        private const string DC_PREFIX = "dc";
        private const string DC_URN = "http://purl.org/dc/elements/1.1/";

        private const string OPF_PREFIX = "opf";
        private const string OPF_URN = "http://www.idpf.org/2007/opf";

        private const string FILE_NAME = "metadata.opf";

        private static readonly XmlWriterSettings DocumentSettings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            IndentChars = "\t",
            ConformanceLevel = ConformanceLevel.Document
        };

        #endregion

        #region Fields

		private readonly Func<string> uniqueIdGenerator;

        #endregion

        #region Constructors and Destructors

        public OpfWriter(Func<string> uniqueIdGenerator)
        {
			this.uniqueIdGenerator = uniqueIdGenerator;
        }

        public OpfWriter() : this(NewUuid)
        {
        }

        #endregion

        #region Public Methods and Operators

        public void WriteTo(string targetPath, Book book, int calibreId, IEnumerable<string> tags)
        {
            var opfPath = Path.Combine(targetPath, FILE_NAME);
            if (!File.Exists(opfPath))
            {
                using (var fileStream = new FileStream(opfPath, FileMode.CreateNew)) 
                    using (var streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false))) 
                        WriteTo(streamWriter, book, calibreId, tags);
            }
        }

        public void WriteTo(TextWriter textWriter, Book book, int calibreId, IEnumerable<string> tags)
        {
            using (var writer = XmlWriter.Create(textWriter, DocumentSettings))
            {
                writer.WriteStartDocument();

                StartPackage(writer);
                StartMetadata(writer);

                WriteIdentifier(writer, "calibre", "calibre_id", calibreId);
                WriteIdentifier(writer, "uuid", "uuid_id", _uniqueIdGenerator.Invoke());

                // Title
                WriteDcElement(writer, "title", book.FullTitle);
                // Authors
                WriteAuthors(writer, book.SortableAuthors, book.Authors);

                WriteCalibreSignature(writer);

                // Date
                WriteDcElement(writer, "date", book.Date);
                // Publisher
                WriteDcElement(writer, "publisher", book.Publisher);
                // Publisher
                WriteDcElement(writer, "language", book.Language);
                // Tags
                WriteTags(writer, tags);

                writer.WriteEndElement();   // Metadata
                writer.WriteEndElement();   // Package

                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        #endregion

        #region Methods

        private static string NewUuid()
        {
            return Guid.NewGuid().ToString();
        }

        private static void StartPackage(XmlWriter writer)
        {
            writer.WriteStartElement("package", OPF_URN);
            writer.WriteAttributeString("unique-identifier", "uuid_id");
            writer.WriteAttributeString("version", "2.0");
        }

        private static void StartMetadata(XmlWriter writer)
        {
            writer.WriteStartElement("metadata");
            writer.WriteAttributeString("xmlns", DC_PREFIX, null, DC_URN);
            writer.WriteAttributeString("xmlns", OPF_PREFIX, null, OPF_URN);
        }

        private static void WriteIdentifier<T>(XmlWriter writer, string scheme, string id, T value)
        {
            writer.WriteStartElement(DC_PREFIX, "identifier", DC_URN);
            writer.WriteAttributeString(OPF_PREFIX, "scheme", OPF_URN, scheme);
            writer.WriteAttributeString("id", id);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        private static void WriteDcElement(XmlWriter writer, string localName, string value)
        {
            writer.WriteElementString(DC_PREFIX, localName, DC_URN, value);
        }

        private static void WriteAuthors(
            XmlWriter writer,
            IEnumerable<string> sortableAuthors,
            IEnumerable<string> allAuthors)
        {
            var authors = string.Join(" & ", sortableAuthors);
            allAuthors.ForEach((_, author) => WriteAuthor(writer, authors, author));
        }

        private static void WriteAuthor(XmlWriter writer, string allSortableAuthors, string author)
        {
            writer.WriteStartElement("creator", DC_URN);
            writer.WriteAttributeString(OPF_PREFIX, "file-as", OPF_URN, allSortableAuthors);
            writer.WriteValue(author);
            writer.WriteEndElement();
        }

        private static void WriteCalibreSignature(XmlWriter writer)
        {
            writer.WriteStartElement(DC_PREFIX, "contributor", DC_URN);
            writer.WriteAttributeString(OPF_PREFIX, "file-as", OPF_URN, "calibre");
            writer.WriteAttributeString(OPF_PREFIX, "role", OPF_URN, "bkp");
            writer.WriteValue("calibre (1.26.0) [http://calibre-ebook.com]");
            writer.WriteEndElement();
        }

        private static void WriteTags(XmlWriter writer, IEnumerable<string> tags)
        {
            tags.ForEach((_, tag) => WriteDcElement(writer, "subject", tag));
        }

        #endregion
    }
}