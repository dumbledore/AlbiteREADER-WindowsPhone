using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albite.Reader.Speech.Narration
{
    public class NarrationSettings
    {
        public static readonly int DefaultHeadingAfterPause = 1000;
        public static readonly int DefaultParagraphAfterPause = 700;
        public static readonly float DefaultSpeedRatio = 1.0f;
        public static readonly float DefaultEmphasisSpeedRatio = 0.7f;
        public static readonly float DefaultQuoteSpeedRatio = 0.85f;

        public int HeadingAfterPause { get; set; }
        public int ParagraphAfterPause { get; set; }
        public float BaseSpeedRatio { get; set; }
        public float EmphasisSpeedRatio { get; set; }
        public float QuoteSpeedRatio { get; set; }

        public NarrationSettings()
        {
            HeadingAfterPause = DefaultHeadingAfterPause;
            ParagraphAfterPause = DefaultParagraphAfterPause;
            BaseSpeedRatio = DefaultSpeedRatio;
            EmphasisSpeedRatio = DefaultEmphasisSpeedRatio;
            QuoteSpeedRatio = DefaultQuoteSpeedRatio;
        }
    }
}
