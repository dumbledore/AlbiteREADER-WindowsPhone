using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.READER.Test;
using SvetlinAnkov.Albite.Tests.Utils.Messaging;
using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Tests
{
    public class Tests : TestCollection
    {
        protected override ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                new UtilsTests(),
                new EpubTests(),
                new TemplateTest("Test/Template/Main.xhtml"),
                new JsonMessengerTest(),
            };
        }
    }
}
