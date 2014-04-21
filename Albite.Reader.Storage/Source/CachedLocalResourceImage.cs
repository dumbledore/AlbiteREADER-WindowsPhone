using Albite.Reader.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Albite.Reader.Storage
{
    internal class CachedLocalResourceImage : CachedResourceImage
    {
        private static string assemblyName;
        private static string getAssemblyName()
        {
            if (assemblyName == null)
            {
                AssemblyName name = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                assemblyName = name.Name;
            }
            return assemblyName;
        }

        public CachedLocalResourceImage(string path) : base("/" + getAssemblyName() + ";component/" + path) { }
    }
}
