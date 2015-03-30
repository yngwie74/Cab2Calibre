namespace Cab2CalibreTests
{
    using Cab2Calibre;

    using NUnit.Framework;

    [TestFixture]
    public class EditionTranslatorTests
    {
        #region Test Methods

        [Test]
        [TestCase(01, "title, 32EX26")] // default: neither 32 nor 26 are edition numbers
        [TestCase(01, "title, 62EBP") ] // default: 62 isn't edition number
        [TestCase(01, "title, 7862E") ] // default: 7862 isn't edition number
        [TestCase(01, "title")]
        [TestCase(01, "title: sub-title")]
        [TestCase(01, "title,   1e: sub-title")]
        [TestCase(02, "title,2e")]
        [TestCase(05, "title, 5E")]
        [TestCase(08, "title,   8e")]
        [TestCase(11, "title,  11e: sub-title")]
        [TestCase(11, "title: sub-title,   11e")]
        [TestCase(13, "title,   13e: sub-title")]
        [TestCase(14, "title, 14e: sub-title")]
        [TestCase(16, "title,   16e: sub-title")]
        [TestCase(19, "title,19E: sub-title")]
        [TestCase(21, "title,   21E: sub-title")]
        [TestCase(30, "title,   30e: sub-title")]
        [TestCase(42, "title, 42E: sub-title")]
        [TestCase(43, "title: sub-title, 43e")]
        [TestCase(47, "title, 47e")]
        [TestCase(50, "title: sub-title, 50e")]
        public void GetNumber(int expected, string title)
        {
            var edition = title.GetEditionNumber();
            Assert.That(edition, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(01, "1st Edition") ][TestCase(02, "2nd Edition") ]
        [TestCase(03, "3rd Edition") ][TestCase(04, "4th Edition") ]
        [TestCase(05, "5th Edition") ][TestCase(06, "6th Edition") ]
        [TestCase(07, "7th Edition") ][TestCase(08, "8th Edition") ]
        [TestCase(09, "9th Edition") ][TestCase(10, "10th Edition")]
        [TestCase(11, "11th Edition")][TestCase(12, "12th Edition")]
        [TestCase(13, "13th Edition")][TestCase(14, "14th Edition")]
        [TestCase(15, "15th Edition")][TestCase(16, "16th Edition")]
        [TestCase(17, "17th Edition")][TestCase(18, "18th Edition")]
        [TestCase(19, "19th Edition")][TestCase(20, "20th Edition")]
        [TestCase(22, "22nd Edition")][TestCase(32, "32nd Edition")]
        [TestCase(34, "34th Edition")][TestCase(48, "48th Edition")]
        [TestCase(55, "55th Edition")][TestCase(56, "56th Edition")]
        [TestCase(63, "63rd Edition")][TestCase(68, "68th Edition")]
        [TestCase(70, "70th Edition")][TestCase(81, "81st Edition")]
        [TestCase(89, "89th Edition")][TestCase(99, "99th Edition")]
        public void InEnglish(int n, string expected)
        {
            var edition = EditionTranslator.GetEdition(n, "eng");
            Assert.That(edition, Is.EqualTo(expected));
        }

        [TestCase(01, "1a Edición") ][TestCase(05, "5a Edición") ]
        [TestCase(05, "5a Edición") ][TestCase(19, "19a Edición")]
        [TestCase(20, "20a Edición")][TestCase(21, "21a Edición")]
        [TestCase(32, "32a Edición")][TestCase(46, "46a Edición")]
        [TestCase(48, "48a Edición")][TestCase(62, "62a Edición")]
        [TestCase(64, "64a Edición")][TestCase(75, "75a Edición")]
        [TestCase(80, "80a Edición")][TestCase(88, "88a Edición")]
        [TestCase(97, "97a Edición")][TestCase(99, "99a Edición")]
        public void InSpanish(int n, string expected)
        {
            var edition = EditionTranslator.GetEdition(n, "spa");
            Assert.That(edition, Is.EqualTo(expected));
        }

        [Test]
        public void InUnknownLanguage()
        {
            var edition = EditionTranslator.GetEdition(7, "xyz");
            Assert.That(edition, Is.EqualTo("7E"));
        }

        [Test]
        [TestCase("title", "title")]
        [TestCase("title,   19E", "title")]
        [TestCase("title,   20e", "title")]
        [TestCase("title,   3e", "title")]
        [TestCase("title,  24E", "title")]
        [TestCase("title,  34e", "title")]
        [TestCase("title,  38e", "title")]
        [TestCase("title, 7e: sub-title", "title: sub-title")]
        [TestCase("title,14e", "title")]
        [TestCase("title,1e: sub-title", "title: sub-title")]
        [TestCase("title,29E", "title")]
        [TestCase("title,31e", "title")]
        [TestCase("title,44e", "title")]
        [TestCase("title,44e: sub-title", "title: sub-title")]
        [TestCase("title,45E", "title")]
        [TestCase("title: sub-title", "title: sub-title")]
        [TestCase("title: sub-title,   42e", "title: sub-title")]
        [TestCase("title: sub-title,  12e", "title: sub-title")]
        [TestCase("title: sub-title,  47E", "title: sub-title")]
        [TestCase("title: sub-title, 2E", "title: sub-title")]
        public void RemoveEdition(string title, string expected)
        {
            var result = title.RemoveEdition();
            Assert.That(result, Is.EqualTo(expected));
        }

        #endregion
    }
}