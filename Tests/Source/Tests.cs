using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.READER.Test;
using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Tests
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
                new JsonMessengerTest(),
            };
        }
    }
}
