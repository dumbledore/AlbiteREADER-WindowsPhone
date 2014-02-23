using Albite.Reader.Core.IO;

namespace Albite.Reader.Engine
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
            using (IsolatedStorage iso = new IsolatedStorage(OutputFilename))
            {
                iso.Write(GetOutput());
            }
        }
    }
}
