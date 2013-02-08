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
    public class TemplateTests : TestCase
    {
        protected override void TestImplementation()
        {
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.MainPage).Test();
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.BaseStyles).Test();
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.ContentStyles).Test();
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.ThemeStyles).Test();
        }
    }
}
