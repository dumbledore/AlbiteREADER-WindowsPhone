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
using SvetlinAnkov.AlbiteREADER.Test.Layout;
using System.Collections.Generic;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public class Tests : TestCollection
    {
        protected override ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                new EpubTests(),
                new TemplateTests(),
                new EngineTest(),
            };
        }
    }
}
