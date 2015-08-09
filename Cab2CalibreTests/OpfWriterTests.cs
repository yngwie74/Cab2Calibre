﻿namespace Cab2CalibreTests
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
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
            "<package unique-identifier=\"uuid_id\" version=\"2.0\" xmlns=\"http://www.idpf.org/2007/opf\">\r\n" +
            "\t<metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:opf=\"http://www.idpf.org/2007/opf\">\r\n" +
            "\t\t<dc:identifier opf:scheme=\"calibre\" id=\"calibre_id\">125</dc:identifier>\r\n" +
            "\t\t<dc:identifier opf:scheme=\"uuid\" id=\"uuid_id\">" + UUID + "</dc:identifier>\r\n" +
            "\t\t<dc:title>¿Quién se ha llevado: mi queso?</dc:title>\r\n" +
            "\t\t<dc:creator opf:file-as=\"Argüeyes, José\">José Argüeyes</dc:creator>\r\n" +
            "\t\t<dc:contributor opf:file-as=\"calibre\" opf:role=\"bkp\">calibre (1.26.0) [http://calibre-ebook.com]</dc:contributor>\r\n" +
            "\t\t<dc:date>1998-01-01T00:00:00-06:00</dc:date>\r\n" +
            "\t\t<dc:publisher>Publisher</dc:publisher>\r\n" +
            "\t\t<dc:language>spa</dc:language>\r\n" +
            "\t\t<dc:subject>tag 1 &amp; 2</dc:subject>\r\n" +
            "\t\t<dc:subject>tag3</dc:subject>\r\n" +
            "\t</metadata>\r\n" +
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