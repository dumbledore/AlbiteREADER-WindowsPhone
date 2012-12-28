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
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Layout
{
    public class LayoutTemplateResource : LayoutTemplate
    {
        private readonly string filename;

        public LayoutTemplateResource(string filename)
        {
            this.filename = filename;

            using (AlbiteResourceStorage res = new AlbiteResourceStorage(filename))
            {
                base.setTemplate(res.ReadAsString());
            }
        }

        public void SaveToStorage()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(filename))
            {
                iso.Write(GetOutput());
            }
        }
    }
}
