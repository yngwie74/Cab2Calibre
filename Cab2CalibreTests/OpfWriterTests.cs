namespace Cab2CalibreTests
{
    using System.IO;

    using Cab2Calibre;

    using NUnit.Framework;

    [TestFixture]
    public class OpfWriterTests
    {
        #region Constants

        private const string UUID = "c1ed58c2-b129-4290-865c-6b4c";

        private const string ExpectedXml1 =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
            "<package unique-identifier=\"uuid_id\" version=\"2.0\" xmlns=\"http://www.idpf.org/2007/opf\">\r\n" +
            "\t<metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:opf=\"http://www.idpf.org/2007/opf\">\r\n" +
            "\t\t<dc:identifier opf:scheme=\"calibre\" id=\"calibre_id\">125</dc:identifier>\r\n" +
            "\t\t<dc:identifier opf:scheme=\"uuid\" id=\"uuid_id\">" + UUID + "</dc:identifier>\r\n" +
            "\t\t<dc:title>¿Quién se ha llevado: mi queso?, 2a Edición</dc:title>\r\n" +
            "\t\t<dc:creator opf:file-as=\"Argüeyes, José\">José Argüeyes</dc:creator>\r\n" +
            "\t\t<dc:contributor opf:file-as=\"calibre\" opf:role=\"bkp\">calibre (1.26.0) [http://calibre-ebook.com]</dc:contributor>\r\n" +
            "\t\t<dc:date>1998-01-01T00:00:00-06:00</dc:date>\r\n" +
            "\t\t<dc:publisher>Publisher</dc:publisher>\r\n" +
            "\t\t<dc:language>spa</dc:language>\r\n" +
            "\t\t<dc:subject>tag 1 &amp; 2</dc:subject>\r\n" +
            "\t\t<dc:subject>tag3</dc:subject>\r\n" +
            "\t</metadata>\r\n" +
            "</package>";

        private const string ExpectedXml2 =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
            "<package unique-identifier=\"uuid_id\" version=\"2.0\" xmlns=\"http://www.idpf.org/2007/opf\">\r\n" +
            "\t<metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:opf=\"http://www.idpf.org/2007/opf\">\r\n" +
            "\t\t<dc:identifier opf:scheme=\"calibre\" id=\"calibre_id\">125</dc:identifier>\r\n" +
            "\t\t<dc:identifier opf:scheme=\"uuid\" id=\"uuid_id\">" + UUID + "</dc:identifier>\r\n" +
            "\t\t<dc:title>iPod &amp; iTunes for Dummies, 3rd Edition</dc:title>\r\n" +
            "\t\t<dc:creator opf:file-as=\"Bove, Tony &amp; Rhodes, Cheryl\">Tony Bove</dc:creator>\r\n" +
            "\t\t<dc:creator opf:file-as=\"Bove, Tony &amp; Rhodes, Cheryl\">Cheryl Rhodes</dc:creator>\r\n" +
            "\t\t<dc:contributor opf:file-as=\"calibre\" opf:role=\"bkp\">calibre (1.26.0) [http://calibre-ebook.com]</dc:contributor>\r\n" +
            "\t\t<dc:date>2005-01-01T00:00:00-06:00</dc:date>\r\n" +
            "\t\t<dc:publisher>Wiley Publishing</dc:publisher>\r\n" +
            "\t\t<dc:language>eng</dc:language>\r\n" +
            "\t\t<dc:subject>Desktop Applications</dc:subject>\r\n" +
            "\t\t<dc:subject>iTunes</dc:subject>\r\n" +
            "\t\t<dc:subject>Multimedia &amp; Interactive</dc:subject>\r\n" +
            "\t</metadata>\r\n" +
            "</package>";

        private const int CALIBRE_ID = 125;
        private const string SourceLine2 = "_DFEA7F1A8E8FAF27E4CFB1810096A8ED.pdf\tiPod & iTunes for Dummies, 3E\t\tBove, Tony|Rhodes, Cheryl\tWiley Publishing\t2005\tIngles";

        #endregion

        #region TestMethods

        [Test]
        public void SpanishWithSubTitleAndEdition()
        {
            AssertGeneratedOpfMatchesSample(
                ExpectedXml1,
                "file\t¿Quién se ha llevado\tmi queso?, 2E\tArgüeyes, José\tPublisher\t1998\tEspañol",
                "tag 1 & 2", "tag3");
        }

        [Test]
        public void EnglishWithEdition()
        {
            AssertGeneratedOpfMatchesSample(
                ExpectedXml2,
                SourceLine2,
                "Desktop Applications", "iTunes", "Multimedia & Interactive");
        }

        [Test]
        public void LineWithoutIsbn()
        {
            const string expectedXml = "<dc:identifier opf:scheme=\"ISBN\">";

            var xml = GenerateOpfDocFor(SourceLine2);
            Assert.That(xml, Does.Not.Contain(expectedXml));
        }

        [Test]
        public void LineWithIsbn()
        {
            const string isbn = "9780134687476";
            const string line = "foo.bar\t\t\t\t\t\tEnglish\t" + isbn;
            const string expectedXml = "<dc:identifier opf:scheme=\"ISBN\">"+isbn+"</dc:identifier>";

            var xml = GenerateOpfDocFor(line);

            Assert.That(xml, Contains.Substring(expectedXml));
        }

        private static void AssertGeneratedOpfMatchesSample(string expectedXml, string line, params string[] tags)
        {
            var xml = GenerateOpfDocFor(line, tags);
            Assert.That(xml, Is.EqualTo(expectedXml));
        }

        private static string GenerateOpfDocFor(string line, params string[] tags)
        {
            var book = new Book(line);
            var st = new StringWriter();
            var opf = new OpfWriter(GenerateBookId);

            opf.WriteTo(st, book, CALIBRE_ID, tags);

            var xml = st.ToString();
            return xml;
        }

        private static string GenerateBookId()
        {
            return UUID;
        }

        #endregion
    }
}