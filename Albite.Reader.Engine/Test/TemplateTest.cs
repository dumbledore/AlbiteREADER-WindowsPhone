using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;

namespace Albite.Reader.Engine.Test
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
            Template t = new TemplateResource(getTemplate());
            listTemplateNames(t);
        }

        private string getTemplate()
        {
            using (ResourceStorage s = new ResourceStorage(filename))
            {
                return s.ReadAsString();
            }
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
