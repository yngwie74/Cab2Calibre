namespace Cab2CalibreTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
	using NUnit.Framework;

    using Cab2Calibre;

    [TestFixture]
    public class BookTests
    {
        [Test]
        public void MainTitle_WithTitle_ShouldReturnTitle()
        {
            var b = new Book("file\tTitle\t\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Title", b.MainTitle);
        }

        [Test]
        public void MainTitle_WithTitleAndSubtitle_ShouldNotIncludeSubtitle()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Title", b.MainTitle);
        }

        [Test]
        public void MainTitle_WithSubtitleAndNoTitle_ShouldReturnSubtitle()
        {
            var b = new Book("file\t\tSubtitle\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Subtitle", b.MainTitle);
        }

        [Test]
        public void MainTitle_WithOtherSuffix_ShouldPreserve()
        {
            var b = new Book("file\tTitle, example\t\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Title, example", b.MainTitle);
        }

        [Test]
        public void MainTitle_WithArticleInTitle_ShouldNotAlterTitle()
        {
            var articles = "A|An|The".Split('|');
            foreach (var article in articles)
            {
                var expected = string.Format("{0} title", article);
                var b = new Book(string.Format("file\t{0}\t\tAuthor\tPublisher\t1998\tEnglish", expected));
                Assert.AreEqual(expected, b.MainTitle, "for article '" + article + "'");
            }
        }

        [Test]
        public void MainTitle_WithArticleAsSubfixInTitle_ShouldChangeToPrefixArticle()
        {
            var articles = "A|An|The".Split('|');
            foreach (var article in articles)
            {
                var title = string.Format("Title, {0}", article);
                var expected = string.Format("{0} Title", article);
                var b = new Book(string.Format("file\t{0}\t\tAuthor\tPublisher\t1998\tEnglish", title));
                Assert.AreEqual(expected, b.MainTitle, "for article '" + article + "'");
            }
        }

        [Test]
        public void SubTitle_WithoutSubtitle_DefaultsToEmpty()
        {
            var b = new Book("file\tTitle\t\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual(string.Empty, b.SubTitle);
        }

        [Test]
        public void SubTitle_WithTitleAndSubtitle_ShouldReturnSubtitle()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Sub", b.SubTitle);
        }

        [Test]
        public void SubTitle_WithSubtitleAndNoTitle_DefaultsToEmpty()
        {
            var b = new Book("file\t\tSubtitle\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual(string.Empty, b.SubTitle);
        }

        [Test]
        public void FullTitle_WithTitle_ShouldIncludeTitle()
        {
            var b = new Book("file\tTitle\t\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Title", b.FullTitle);
        }

        [Test]
        public void FullTitle_WithTitleAndSubitle_ShouldIncludeSubtitle()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Title: Sub", b.FullTitle);
        }

        [Test]
        public void SortableAuthors_WithoutAuthors_DefaultsToEmpty()
        {
            var b = new Book("file\tTitle\tSub\t\tPublisher\t1998\tEnglish");
            Assert.AreEqual(0, b.SortableAuthors.Count(), "item count");
        }

        [Test]
        public void SortableAuthors_WithSingleAuthor_ShouldIncludeAuthor()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Author", b.SortableAuthors.Single());
        }

        [Test]
        public void SortableAuthors_WithTwoAuthors_ShouldIncludeBoth()
        {
            var b = new Book("file\tTitle\tSub\tAuthor|Contributor\tPublisher\t1998\tEnglish");
            var sa = b.SortableAuthors.ToArray();
            Assert.AreEqual("Author", sa[0], "first");
            Assert.AreEqual("Contributor", sa[1], "second");
        }

        [Test]
        public void SortableAuthors_AuthorsHaveRole_ShouldStripRole()
        {
            var b = new Book("file\tTitle\tSub\tJohn|Pete  (Contributor)|Jack(Editor)\tPublisher\t1998\tEnglish");
            var sa = b.SortableAuthors.ToArray();
            Assert.AreEqual("John", sa[0], "first");
            Assert.AreEqual("Pete", sa[1], "second");
            Assert.AreEqual("Jack", sa[2], "third");
        }

        [Test]
        public void Authors_WithSingleAuthor_ShouldIncludeAuthor()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Author", b.Authors.Single());
        }

        [Test]
        public void Authors_WithTwoAuthors_ShouldIncludeBoth()
        {
            var b = new Book("file\tTitle\tSub\tAuthor|Contributor\tPublisher\t1998\tEnglish");
            var sa = b.Authors.ToArray();
            Assert.AreEqual("Author", sa[0], "first");
            Assert.AreEqual("Contributor", sa[1], "second");
        }

        [Test]
        public void Authors_AuthorsHaveRole_ShouldStripRole()
        {
            var b = new Book("file\tTitle\tSub\tJohn|Pete  (Contributor)|Jack(Editor)\tPublisher\t1998\tEnglish");
            var sa = b.Authors.ToArray();
            Assert.AreEqual("John", sa[0], "first");
            Assert.AreEqual("Pete", sa[1], "second");
            Assert.AreEqual("Jack", sa[2], "third");
        }

        [Test]
        public void Authors_IfSurnameComesFirst_ShouldReverseOrder()
        {
            var b = new Book("file\tTitle\tSub\tSmith, John\tPublisher\t1998\tEnglish");
            Assert.AreEqual("John Smith", b.Authors.Single());
        }

        [Test]
        public void MainAuthor_WithoutAuthors_DefaultsToUnknown()
        {
            var b = new Book("file\tTitle\tSub\t\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Unknown", b.MainAuthor);
        }

        [Test]
        public void MainAuthors_WithSingleAuthor_ShouldReturnAuthor()
        {
            var b = new Book("file\tTitle\tSub\tSmith, John\tPublisher\t1998\tEnglish");
            Assert.AreEqual("John Smith", b.MainAuthor);
        }

        [Test]
        public void MainAuthor_WithVariousAuthors_ShouldReturnFirst()
        {
            var b = new Book("file\tTitle\tSub\tBrown, Charles|Parker, Peter|Sparrow, Jack\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Charles Brown", b.MainAuthor);
        }

        [Test]
        public void Publisher_WithoutPublisher_DefaultsToEmpty()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\t\t1998\tEnglish");
            Assert.AreEqual(string.Empty, b.Publisher);
        }

        [Test]
        public void Publisher_WithPublisher_ShouldReturnPublisher()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual("Publisher", b.Publisher);
        }

        [Test]
        public void Date_WithoutYear_DefaultsToEmpty()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\t\t\tEnglish");
            Assert.AreEqual(string.Empty, b.Date);
        }

        [Test]
        public void Date_WithInvalidYear_DefaultsToEmpty()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tYEAR\t\tEnglish");
            Assert.AreEqual(string.Empty, b.Date);
        }

        [Test]
        public void Date_WithValidYear_ShouldReturnDateWithTimeZone()
        {
            var b = new Book("file\tTitle\tSub\tAuthor\tDate\t1998\tEnglish");
            Assert.AreEqual("1998-01-01T00:00:00-06:00", b.Date);
        }

        [Test]
        public void Language_WithoutLanguage_DefaultsToEnglish()
        {
            const string Expected = "eng";

            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998");
            Assert.AreEqual(Expected, b.Language, "missing element");

            b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\t");
            Assert.AreEqual(Expected, b.Language, "empty element");
        }

        [Test]
        public void Language_WithUnknownLanguage_DefaultsToEnglish()
        {
            const string Expected = "eng";

            var b = new Book("file\tTitle\tSub\tAuthor\tPublisher\t1998\tMexicano");
            Assert.AreEqual(Expected, b.Language);
        }

        [Test]
        public void Language_WithKnownLanguage_ShouldReturnIsoCode()
        {
            var langs = LanguageCatalog.Items;

            var funcs = new Dictionary<string, Func<string, string>>
                            {
                                { "identity", s => s },
                                { "ToLower", s => s.ToLower() },
                                { "ToUpper", s => s.ToUpper() },
                            };

            foreach (var lang in langs)
            {
                foreach (var func in funcs)
                {
                    var b = new Book(string.Format("file\tTitle\tSub\tAuthor\tPublisher\t1998\t{0}", func.Value(lang.Key)));

                    var message = string.Format("using {0}('{1}')", func.Key, lang.Key);
                    Assert.AreEqual(lang.Value, b.Language, message);
                }
            }
        }

        [Test]
        public void DirNameInLib_WithoutSubtitle_ShouldIncludeAuthorTitleAndId()
        {
            var b = new Book("file\tTitle\t\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual(@"Author" + Path.DirectorySeparatorChar + @"Title (25)", b.DirNameInLib(25));
        }

        [Test]
        public void DirNameInLib_WithSubtitle_ShouldIncludeAuthorFullTitleAndId()
        {
            var b = new Book("file\tTitle\tSubtitle\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual(@"Author" + Path.DirectorySeparatorChar + @"Title_ Subtitle (25)", b.DirNameInLib(25));
        }

        [Test]
#if __MonoCS__
        [Ignore("Los caracteres inválidos son diferentes en linux")]
#endif
        public void DirNameInLib_ShouldRemoveInvalidChars()
        {
            var b = new Book("file\t¿Quién se ha llevado\tmi queso?\tArgüeyes, José\tPublisher\t1998\tEspañol");
            Assert.AreEqual(@"Jose Argueyes" + Path.DirectorySeparatorChar + @"Quien se ha llevado_ mi queso (25)", b.DirNameInLib(25));
        }

        [Test]
        public void DirNameInLib_AtMost50CharsPerSegment()
        {
            var b =
                new Book(
                    "file\t" + "Title6789012345678901234567890\t" + "Subtitle9012345678901234567890\t"
                    + "Author789012345678901234567890123456789012345678901234567890\t" + "Publisher\t1998\tEnglish");

            var parts = b.DirNameInLib(2857).Split(Path.DirectorySeparatorChar);
            foreach (var part in parts)
            {
                Assert.IsTrue(part.Length <= 50, "Path segment too long: '" + part + "'.");
            }
        }

        [Test]
        public void FileNameInLib_WithoutSubtitle_ShouldIncludeMainTitleAndAuthorOnly()
        {
            var b = new Book("file.epub\tTitle\t\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual(@"Title - Author.epub", b.FileNameInLib);
        }

        [Test]
        public void FileNameInLib_WithSubtitle_ShouldIncludeMainTitleAndAuthorOnly()
        {
            var b = new Book("file.epub\tTitle\tSubtitle\tAuthor\tPublisher\t1998\tEnglish");
            Assert.AreEqual(@"Title - Author.epub", b.FileNameInLib);
        }

        [Test]
#if __MonoCS__
        [Ignore("Los caracteres inválidos son diferentes en linux")]
#endif
        public void FileNameInLib_ShouldRemoveInvalidChars()
        {
            var b = new Book("file.epub\t¿Quién se ha llevado mi queso?\t\tArgüeyes, José\tPublisher\t1998\tEspañol");
            Assert.AreEqual(@"Quien se ha llevado mi queso_ - Jose Argueyes.epub", b.FileNameInLib);
        }

        [Test]
        public void FileNameInLib_AtMost50CharsLong()
        {
            var b = new Book(
                "file.epub\t" + 
                "Title6789012345678901234567890\t" + 
                "Subtitle9012345678901234567890\t" + 
                "Author789012345678901234567890123456789012345678901234567890\t" +
                "Publisher\t" +
                "1998\tEnglish");

            var baseFileName = Path.GetFileNameWithoutExtension(b.FileNameInLib);

            Assert.IsNotNull(baseFileName, "File name is null!");

            Assert.IsTrue(
                baseFileName.Length <= 50,
                string.Format("FileNameInLib too long: '{0}'.", b.FileNameInLib));
        }

        [Test]
        public void ToString_ShouldIncludeFullTitleAndMainAuthor()
        {
            const string Expected = "Building Open Source Tools: Components and Techniques by Mike D. Schiffman";

            var b = new Book(
                "file.epub\t" + 
                "Building Open Source Tools\t" + 
                "Components and Techniques\t" + 
                "Schiffman, Mike D.\t" + 
                "John Wiley & Sons\t" + 
                "2003\tInglés");

            Assert.AreEqual(Expected, b.ToString());
        }
    }
}