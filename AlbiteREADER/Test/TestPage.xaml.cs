using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Diagnostics;
using System.Windows.Resources;
using SvetlinAnkov.AlbiteREADER.Layout;
using SvetlinAnkov.AlbiteREADER.Utils;
using SvetlinAnkov.AlbiteREADER.Test.Layout;
using SvetlinAnkov.AlbiteREADER.Test.Model.Container;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();

            runTests();
        }

        private void runTests()
        {
            // Layout Templates
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.MainPage).Test();
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.BaseStyles).Test();
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.ContentStyles).Test();
            new TemplateTest(Defaults.Engine.LayoutPath + Defaults.Engine.ThemeStyles).Test();

            // Layout Engine
            new EngineTest().Test();

            // Epub Container
            new EpubContainerTest("Test/aliceDynamic.epub").Test();
            new EpubContainerTest("Test/idpf.epub").Test();
        }
    }
}