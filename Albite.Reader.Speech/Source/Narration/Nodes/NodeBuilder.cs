using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albite.Reader.Speech.Narration.Nodes
{
    internal abstract class Builder
    {
        private StringBuilder b = new StringBuilder();

        public Builder Append(string s)
        {
            b.Append(s);
            return this;
        }

        public Builder Append(float f)
        {
            b.Append(f);
            return this;
        }

        public Builder Append(int i)
        {
            b.Append(i);
            return this;
        }

        public Builder Append(object o)
        {
            b.Append(o);
            return this;
        }

        public override string ToString()
        {
            return b.ToString();
        }
    }
}
