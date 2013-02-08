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

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public class Tests : TestCase
    {
        protected override void TestImplementation()
        {
            new EpubTests().Test();
            new TemplateTests().Test();
            new EngineTest().Test();
        }
    }
}
