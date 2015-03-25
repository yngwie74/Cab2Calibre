namespace Cab2CalibreTests
{
    using System.IO;

    using Cab2Calibre;

    using NUnit.Framework;

    [TestFixture]
    public class CabinetTests
    {
        #region Static Fields

        private static readonly string Biblioteca = Path.Combine(".", "Biblioteca");
        private static readonly string DirName = Path.Combine(Biblioteca, "Informatica", "Programacion");

        #endregion

        #region Test Methods

        [SetUp]
        public static void SetUp()
        {
            Directory.CreateDirectory(DirName);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(Biblioteca))
            {
                Directory.Delete(Biblioteca, true);
            }
        }

        [Test]
        public void Title()
        {
            using (File.CreateText(Path.Combine(DirName, "articles.idx")))
            {
                var c = new Cabinet(DirName);
                Assert.That(c.Title, Is.EqualTo("Programacion"));
            }
        }

        #endregion
    }
}