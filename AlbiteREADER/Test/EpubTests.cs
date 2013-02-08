using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SvetlinAnkov.AlbiteREADER.Test.Model.Container;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public class EpubTests : TestCase
    {
        protected override void TestImplementation()
        {
            invalidEpubContainerTestsThatFail();
            invalidEpubContainerTestsThatPass();
            validEpubContainerTests();
        }

        /// <summary>
        /// These epubs should fail to load with an appropriate
        /// exception being thrown
        /// </summary>
        private void invalidEpubContainerTestsThatFail()
        {
            // A non existent resource
            new InvalidEpubContainerTest("Test/epub/invalid/doesntexist").Test();

            // An empty zip file
            new InvalidEpubContainerTest("Test/epub/invalid/fail/01.epub").Test();

            // There is a META-INF/container.xml, but it doesn't have a rootfile element
            new InvalidEpubContainerTest("Test/epub/invalid/fail/02.epub").Test();

            // There is a META-INF/container.xml, the rootfile element is missing
            // the full-path attribute
            new InvalidEpubContainerTest("Test/epub/invalid/fail/03.epub").Test();

            // There is a valid META-INF/container.xml, but nothing else
            new InvalidEpubContainerTest("Test/epub/invalid/fail/04.epub").Test();

            // No manifest in the OPF
            new InvalidEpubContainerTest("Test/epub/invalid/fail/05.epub").Test();

            // No spine in the OPF
            new InvalidEpubContainerTest("Test/epub/invalid/fail/06.epub").Test();
        }

        /// <summary>
        /// These epubs are not valid and should produce error
        /// messages, but should pass
        /// </summary>
        private void invalidEpubContainerTestsThatPass()
        {
            // No metadata. The root element is misspelled
            new EpubContainerTest("Test/epub/invalid/pass/01.epub").Test();

            // Manifest has items without an id or a href
            // Spine doesn't have a toc attribute, so no ncx would be loaded.
            // Spine has an element without a idref
            new EpubContainerTest("Test/epub/invalid/pass/02.epub").Test();

            // The ncx item is missing even though the spine element has a toc attribute
            // The metadata is empty
            new EpubContainerTest("Test/epub/invalid/pass/03.epub").Test();

            // The ncx file is missing
            new EpubContainerTest("Test/epub/invalid/pass/04.epub").Test();

            // No NavMap and NavLists in the ncx
            new EpubContainerTest("Test/epub/invalid/pass/05.epub").Test();

            // A misspelled root element.

            // A NavPoint without a content element.
            // A NavPoint with content but without a src attribute.
            // A NavPoint without a label element.
            // A NavPoint with a label but without a text element.

            // A NavList without a label element
            // A NavList with a label but without a text element.

            // A NavTarget without a content element.
            // A NavTarget with content but without a src attribute.
            // A NavTarget without a label element.
            // A NavTarget with a label but without a text element.
            new EpubContainerTest("Test/epub/invalid/pass/06.epub").Test();

            // Has guide references
            // A guide ref with an unknown type
            // A guide ref with an empty string for a type
            // A guide ref without a type
            // A guide ref without a title
            // A guide ref without a href
            // A guide ref without attributes at all
            new EpubContainerTest("Test/epub/invalid/pass/07.epub").Test();
        }

        /// <summary>
        /// These epubs should not fail and produce no error messages
        /// </summary>
        private void validEpubContainerTests()
        {
            new EpubContainerTest("Test/epub/aliceDynamic.epub").Test();
            new EpubContainerTest("Test/epub/idpf.epub").Test();
        }
    }
}
