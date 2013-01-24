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
using System.Diagnostics;

namespace SvetlinAnkov.AlbiteREADER.Test.Layout
{
    public class TemplateTest : AlbiteTest
    {
        private string filename;

        public TemplateTest(string filename)
        {
            this.filename = filename;
        }

        protected override void TestImplementation()
        {
            Debug.WriteLine("Opening Template {0}", filename);
            Template t = new TemplateResource(filename);
            listTemplateNames(t);
        }

        private void listTemplateNames(Template t)
        {
            Debug.WriteLine("Number of placeholder names: {0}", t.Count);
            foreach (string name in t.Names)
            {
                Debug.WriteLine("Name: {0} = {1}", name, t[name] != null ? t[name] : "<null>");
            }
        }
    }
}
