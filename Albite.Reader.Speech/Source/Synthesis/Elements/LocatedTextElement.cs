using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class LocatedTextElement<TLocation> : TextElement
    {
        public TLocation Location { get; private set; }

        public LocatedTextElement(int id, string text, TLocation location)
            : base(id, text)
        {
            Location = location;
        }
    }
}
