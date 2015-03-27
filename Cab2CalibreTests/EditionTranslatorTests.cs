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
            var edition = EditionTranslator.GetNumber(title);
            Assert.That(edition, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(",1E", "1st Edition")  ][TestCase(",2E", "2nd Edition")  ]
        [TestCase(",3E", "3rd Edition")  ][TestCase(",4E", "4th Edition")  ]
        [TestCase(",5E", "5th Edition")  ][TestCase(",6E", "6th Edition")  ]
        [TestCase(",7E", "7th Edition")  ][TestCase(",8E", "8th Edition")  ]
        [TestCase(",9E", "9th Edition")  ][TestCase(",10E", "10th Edition")]
        [TestCase(",11E", "11th Edition")][TestCase(",12E", "12th Edition")]
        [TestCase(",13E", "13th Edition")][TestCase(",14E", "14th Edition")]
        [TestCase(",15E", "15th Edition")][TestCase(",16E", "16th Edition")]
        [TestCase(",17E", "17th Edition")][TestCase(",18E", "18th Edition")]
        [TestCase(",19E", "19th Edition")][TestCase(",20E", "20th Edition")]
        [TestCase(",22E", "22nd Edition")][TestCase(",32E", "32nd Edition")]
        [TestCase(",34E", "34th Edition")][TestCase(",48E", "48th Edition")]
        [TestCase(",55E", "55th Edition")][TestCase(",56E", "56th Edition")]
        [TestCase(",63E", "63rd Edition")][TestCase(",68E", "68th Edition")]
        [TestCase(",70E", "70th Edition")][TestCase(",81E", "81st Edition")]
        [TestCase(",89E", "89th Edition")][TestCase(",99E", "99th Edition")]
        public void InEnglish(string title, string expected)
        {
            var edition = EditionTranslator.GetEdition(title, "eng");
            Assert.That(edition, Is.EqualTo(expected));
        }

        [TestCase(",3E", "3a Edición")  ][TestCase(",5E", "5a Edición")  ]
        [TestCase(",5E", "5a Edición")  ][TestCase(",19E", "19a Edición")]
        [TestCase(",20E", "20a Edición")][TestCase(",21E", "21a Edición")]
        [TestCase(",32E", "32a Edición")][TestCase(",46E", "46a Edición")]
        [TestCase(",48E", "48a Edición")][TestCase(",62E", "62a Edición")]
        [TestCase(",64E", "64a Edición")][TestCase(",75E", "75a Edición")]
        [TestCase(",80E", "80a Edición")][TestCase(",88E", "88a Edición")]
        [TestCase(",97E", "97a Edición")][TestCase(",99E", "99a Edición")]
        public void InSpanish(string title, string expected)
        {
            var edition = EditionTranslator.GetEdition(title, "spa");
            Assert.That(edition, Is.EqualTo(expected));
        }

        [Test]
        public void InUnknownLanguage()
        {
            var edition = EditionTranslator.GetEdition("title, 7e: sub-title", "xyz");
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
            var result = EditionTranslator.RemoveEdition(title);
            Assert.That(result, Is.EqualTo(expected));
        }

        #endregion
    }
}