using Albite.Reader.Speech.Narration.Commands;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlNarrationExpression : NarrationExpression
    {
        public XhtmlLocation Location { get; private set; }

        public XhtmlNarrationExpression(string text, XhtmlLocation location)
            : base(text)
        {
            Location = location;
        }

        public override string ToString()
        {
            return base.ToString() + " at " + Location;
        }
    }
}
