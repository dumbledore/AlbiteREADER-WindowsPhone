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
using SvetlinAnkov.AlbiteREADER.Layout;
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Test.Layout
{
    public class EngineTest : AlbiteTest
    {
        protected override void TestImplementation()
        {
            Engine engine = new Engine(null, Defaults.Layout.DefaultSettings);
        }
    }
}
