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
using SvetlinAnkov.AlbiteREADER.Model.Containers;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public partial class TestBookPage : PhoneApplicationPage
    {
        public TestBookPage()
        {
            InitializeComponent();

            Engine engine = new Engine(null, Defaults.Layout.DefaultSettings);

            string epubPath = "Test/aliceDynamic.epub";
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(epubPath))
            {
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(epubPath))
                {
                    res.CopyTo(iso);
                }

                using (Stream inputStream = iso.ReadAsStream())
                {
                    using (AlbiteZipContainer zip = new AlbiteZipContainer(inputStream))
                    {
                        using (AlbiteIsolatedStorage outputStorage = new AlbiteIsolatedStorage("Test/epub/"))
                        {
                            EpubContainer epub = new EpubContainer(zip);
                            epub.Install(outputStorage);
                        }
                    }
                }
            }
        }

        private void listTemplateNames(Template t)
        {
            Debug.WriteLine("Number of placeholder names: {0}", t.Count);
            foreach (string name in t.Names)
            {
                Debug.WriteLine("Name: {0} = {1}", name, t[name]);
            }
        }
    }
}