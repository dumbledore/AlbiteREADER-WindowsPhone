using System;
using System.Collections.Generic;
using System.Linq;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlLocation
    {
        public IEnumerable<int> ElementPath { get; private set; }

        public XhtmlLocation(IEnumerable<int> elementPath)
        {
            if (elementPath == null)
            {
                throw new NullReferenceException();
            }

            ElementPath = elementPath;
        }

        public override bool Equals(object obj)
        {
            if (obj is XhtmlLocation)
            {
                XhtmlLocation other = (XhtmlLocation)obj;
                return Enumerable.SequenceEqual<int>(ElementPath, other.ElementPath);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ElementPath.GetHashCode();
        }
    }
}
