using Albite.Reader.Core.Test;
using Albite.Reader.Engine.Test;
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
                new JsonSerializerTest(),
            };
        }
    }
}
