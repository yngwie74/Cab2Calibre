namespace Cab2CalibreTests
{
    using System;
    using System.IO;

    using Cab2Calibre;

    using NUnit.Framework;

    [TestFixture]
    public class OpfWriterTests
    {
        #region Constants

        private const string UUID = "c1ed58c2-b129-4290-865c-6b4c";

        private static readonly string ExpectedXml =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" + Environment.NewLine +
            "<package unique-identifier=\"uuid_id\" version=\"2.0\" xmlns=\"http://www.idpf.org/2007/opf\">" + Environment.NewLine +
            "\t<metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:opf=\"http://www.idpf.org/2007/opf\">" + Environment.NewLine +
            "\t\t<dc:identifier opf:scheme=\"calibre\" id=\"calibre_id\">125</dc:identifier>" + Environment.NewLine +
            "\t\t<dc:identifier opf:scheme=\"uuid\" id=\"uuid_id\">" + UUID + @"</dc:identifier>" + Environment.NewLine +
            "\t\t<dc:title>¿Quién se ha llevado: mi queso?</dc:title>" + Environment.NewLine +
            "\t\t<dc:creator opf:file-as=\"Argüeyes, José\">José Argüeyes</dc:creator>" + Environment.NewLine +
            "\t\t<dc:contributor opf:file-as=\"calibre\" opf:role=\"bkp\">calibre (1.26.0) [http://calibre-ebook.com]</dc:contributor>" + Environment.NewLine +
            "\t\t<dc:date>1998-01-01T00:00:00-06:00</dc:date>" + Environment.NewLine +
            "\t\t<dc:publisher>Publisher</dc:publisher>" + Environment.NewLine +
            "\t\t<dc:language>spa</dc:language>" + Environment.NewLine +
            "\t\t<dc:subject>tag 1 &amp; 2</dc:subject>" + Environment.NewLine +
            "\t\t<dc:subject>tag3</dc:subject>" + Environment.NewLine +
            "\t</metadata>" + Environment.NewLine +
            "</package>";

        #endregion

        #region Public Methods and Operators

        [Test]
        public void WriteOpf()
        {
            Func<string> idGenerator = () => UUID;

            var book = new Book("file\t¿Quién se ha llevado\tmi queso?\tArgüeyes, José\tPublisher\t1998\tEspañol");
            var st = new StringWriter();
            var opf = new OpfWriter(idGenerator);

            opf.WriteTo(st, book, 125, new[] { "tag 1 & 2", "tag3" });

            var xml = st.ToString();

            Assert.That(xml, Is.EqualTo(ExpectedXml));
        }

        #endregion
    }
}