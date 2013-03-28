using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Tests.Test.Utils;

namespace SvetlinAnkov.Albite.Tests.Test
{
    public class UtilsTests : TestCollection
    {
        protected override System.Collections.Generic.ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                //new AlbiteIsolatedStorageTest("Test/epub/aliceDynamic.epub"),
                new AlbiteIsolatedContainerTestWrapper(
                    "Test/epub/aliceDynamic.epub",
                    new string[] {"chapter01.xhtml", "Thumbnails/thumbnail.png", "META-INF/container.xml"},
                    new string[] {"chapter01.xhtml", "Thumbnails/thumbnail.png", "META-INF/container.xml"}),
                new TestFailWrapper(
                    new AlbiteIsolatedContainerTestWrapper(
                        "Test/epub/aliceDynamic.epub",
                        new string[] {"chapter01.xhtml"},
                        new string[] {"chapter01.xhtml", "/home/root/file"}),
                    typeof(System.IO.FileNotFoundException)),
                new TestFailWrapper(
                    new AlbiteIsolatedContainerTestWrapper(
                        "Test/epub/aliceDynamic.epub",
                        new string[] {"chapter01.xhtml"},
                        new string[] {"chapter01.xhtml", "mydata/../../othersdata"}),
                    typeof(System.IO.FileNotFoundException)),
                new TestFailWrapper(
                    new AlbiteIsolatedContainerTestWrapper(
                        "Test/epub/aliceDynamic.epub",
                        new string[] {"chapter01.xhtml"},
                        new string[] {"chapter01.xhtml", "/root/home/mydata/../../../notmine"}),
                    typeof(System.IO.FileNotFoundException)),
            };
        }
    }
}
