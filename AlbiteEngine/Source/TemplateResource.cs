using SvetlinAnkov.Albite.Core.IO;

namespace SvetlinAnkov.Albite.Engine
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
