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
using SvetlinAnkov.Albite.READER.BrowserEngine;
using SvetlinAnkov.Albite.Core.Test;

namespace SvetlinAnkov.Albite.Tests.Test.BrowserEngine
{
    public class TemplateTest : TestCase
    {
        private string filename;

        public TemplateTest(string filename)
        {
            this.filename = filename;
        }

        protected override void TestImplementation()
        {
            Log("Opening Template {0}", filename);
            Template t = new TemplateResource(filename);
            listTemplateNames(t);
        }

        private void listTemplateNames(Template t)
        {
            Log("Number of placeholder names: {0}", t.Count);
            foreach (string name in t.Names)
            {
                Log("Name: {0} = {1}", name, t[name] != null ? t[name] : "<null>");
            }
        }
    }
}
