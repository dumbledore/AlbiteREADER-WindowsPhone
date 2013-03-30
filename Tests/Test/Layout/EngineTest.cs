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
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.READER.Utils;
using SvetlinAnkov.Albite.READER.Layout;

namespace SvetlinAnkov.Albite.Tests.Test.Layout
{
    public class EngineTest : TestCase
    {
        protected override void TestImplementation()
        {
            BrowserEngine engine = new BrowserEngine(null, Defaults.Layout.DefaultSettings);
        }
    }
}
