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

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public class TemplateResource : Template
    {
        public string OutputFilename { get; set; }

        public TemplateResource(string template) : base(template) { }

        public TemplateResource(string template, string outputFilename)
            : this(template)
        {
            OutputFilename = outputFilename;
        }

        public void SaveToStorage()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(OutputFilename))
            {
                iso.Write(GetOutput());
            }
        }
    }
}
