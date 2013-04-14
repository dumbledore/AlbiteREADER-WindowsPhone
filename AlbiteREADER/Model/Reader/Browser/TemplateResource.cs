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

namespace SvetlinAnkov.Albite.READER.Model.Reader.Browser
{
    public class TemplateResource : Template
    {
        private readonly string inputFilename;
        private readonly string outputFilename;

        public TemplateResource(string filename) : this(filename, filename) { }

        public TemplateResource(string inputFilename, string outputFilename)
        {
            this.inputFilename = inputFilename;
            this.outputFilename = outputFilename;

            using (AlbiteResourceStorage res = new AlbiteResourceStorage(inputFilename))
            {
                base.setTemplate(res.ReadAsString());
            }
        }

        public void SaveToStorage()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(outputFilename))
            {
                iso.Write(GetOutput());
            }
        }
    }
}
