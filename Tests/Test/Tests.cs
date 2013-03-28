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
using System.Collections.Generic;
using SvetlinAnkov.Albite.Tests.Test.Layout;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Tests.Test.Utils;

namespace SvetlinAnkov.Albite.Tests.Test
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
                new EngineTest(),
            };
        }
    }
}
