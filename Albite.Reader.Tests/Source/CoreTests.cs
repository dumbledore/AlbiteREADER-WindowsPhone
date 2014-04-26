using Albite.Reader.Core.Test;

namespace Albite.Reader.Tests
{
    public class CoreTests : TestCollection
    {
        protected override System.Collections.Generic.ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                new CircularBufferTest(1024),
                new TreeEnumeratorTest(),
                new RecordStoreTestWrapper("Test/recordstore.sdf"),
                new IsolatedStorageTest("Test/epub/aliceDynamic.epub"),
                new IsolatedContainerTestWrapper(
                    "Test/epub/aliceDynamic.epub",
                    new string[] {"chapter01.xhtml", "Thumbnails/thumbnail.png", "META-INF/container.xml"},
                    new string[] {"chapter01.xhtml", "Thumbnails/thumbnail.png", "META-INF/container.xml"}),
                new TestFailWrapper(
                    new IsolatedContainerTestWrapper(
                        "Test/epub/aliceDynamic.epub",
                        new string[] {"chapter01.xhtml"},
                        new string[] {"chapter01.xhtml", "/home/root/file"}),
                    typeof(System.IO.FileNotFoundException)),
                new TestFailWrapper(
                    new IsolatedContainerTestWrapper(
                        "Test/epub/aliceDynamic.epub",
                        new string[] {"chapter01.xhtml"},
                        new string[] {"chapter01.xhtml", "mydata/../../othersdata"}),
                    typeof(System.IO.FileNotFoundException)),
                new TestFailWrapper(
                    new IsolatedContainerTestWrapper(
                        "Test/epub/aliceDynamic.epub",
                        new string[] {"chapter01.xhtml"},
                        new string[] {"chapter01.xhtml", "/root/home/mydata/../../../notmine"}),
                    typeof(System.IO.FileNotFoundException)),
                new JsonSerializerTest(),
            };
        }
    }
}
