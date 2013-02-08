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
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public class TemplateTests : TestCollection
    {
        protected override System.Collections.Generic.ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.MainPage),
                new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.BaseStyles),
                new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.ContentStyles),
                new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.ThemeStyles),
            };
        }
    }
}
