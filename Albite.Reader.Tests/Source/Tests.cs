using Albite.Reader.Core.Test;
using Albite.Reader.Engine.Test;
using Albite.Reader.Speech.Test;
using System.Collections.Generic;

namespace Albite.Reader.Tests
{
    public class Tests : TestCollection
    {
        protected override ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                new CoreTests(),
                new EpubTests(),
                new TemplateTest("Test/Template/Main.xhtml"),
                new XhtmlParserTest("Test/Speech/down-the-rabbit-hole.xhtml"),
            };
        }
    }
}
