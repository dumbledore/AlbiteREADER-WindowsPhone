using System;
using System.Collections.Generic;

namespace Albite.Reader.Speech.Narration
{
    public abstract class Narrator<TExpression> where TExpression : NarrationExpression
    {
        protected INarrationCommand Root { get; private set; }
        protected INarrationCommand Current { get; private set; }

        protected Narrator(INarrationCommand root)
        {
            Root = root;
            Current = root;
        }
    }
}
