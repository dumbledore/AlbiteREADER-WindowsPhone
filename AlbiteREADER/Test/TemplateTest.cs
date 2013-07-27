using SvetlinAnkov.Albite.READER.Model.Reader;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.READER.Test
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
            using (AlbiteResourceStorage s = new AlbiteResourceStorage(filename))
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
