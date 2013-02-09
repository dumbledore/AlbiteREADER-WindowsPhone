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
    public class EpubTests : TestCollection
    {
        protected override System.Collections.Generic.ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                // ---------------------------------------------------
                // These epubs should fail to load with an appropriate
                // exception being thrown

                // A non existent resource
                new InvalidEpubContainerTest("Test/Epub/Invalid/doesntexist"),

                // An empty zip file
                new InvalidEpubContainerTest("Test/Epub/Invalid/Fail/01.epub"),

                // There is a META-INF/container.xml, but it doesn't have a rootfile element
                new InvalidEpubContainerTest("Test/Epub/Invalid/Fail/02.epub"),

                // There is a META-INF/container.xml, the rootfile element is missing
                // the full-path attribute
                new InvalidEpubContainerTest("Test/Epub/Invalid/Fail/03.epub"),

                // There is a valid META-INF/container.xml, but nothing else
                new InvalidEpubContainerTest("Test/Epub/Invalid/Fail/04.epub"),

                // No manifest in the OPF
                new InvalidEpubContainerTest("Test/Epub/Invalid/Fail/05.epub"),

                // No spine in the OPF
                new InvalidEpubContainerTest("Test/Epub/Invalid/Fail/06.epub"),

                // --------------------------------------------------
                // These epubs are not valid and should produce error
                // messages, but should pass

                // No metadata. The root element is misspelled
                new EpubContainerTest("Test/Epub/Invalid/Pass/01.epub"),

                // Manifest has items without an id or a href
                // Spine doesn't have a toc attribute, so no ncx would be loaded.
                // Spine has an element without a idref
                new EpubContainerTest("Test/Epub/Invalid/Pass/02.epub"),

                // The ncx item is missing even though the spine element has a toc attribute
                // The metadata is empty
                new EpubContainerTest("Test/Epub/Invalid/Pass/03.epub"),

                // The ncx file is missing
                new EpubContainerTest("Test/Epub/Invalid/Pass/04.epub"),

                // No NavMap and NavLists in the ncx
                new EpubContainerTest("Test/Epub/Invalid/Pass/05.epub"),

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
                new EpubContainerTest("Test/Epub/Invalid/Pass/06.epub"),

                // Has guide references
                // A guide ref with an unknown type
                // A guide ref with an empty string for a type
                // A guide ref without a type
                // A guide ref without a title
                // A guide ref without a href
                // A guide ref without attributes at all
                new EpubContainerTest("Test/Epub/Invalid/Pass/07.epub"),

                // Items with dangerous paths and invalid characters
                new EpubContainerTest("Test/Epub/Invalid/Pass/08.epub"),

                // ---------------------------------------------------------
                // These epubs should not fail and produce no error messages
                new EpubContainerTest("Test/epub/aliceDynamic.epub"),
                new EpubContainerTest("Test/epub/idpf.epub"),
            };
        }
    }
}
