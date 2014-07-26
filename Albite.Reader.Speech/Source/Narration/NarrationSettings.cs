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
        public static readonly int DefaultEmphasisPause = 250;
        public static readonly int DefaultEmphasisPitch = -4;
        public static readonly float DefaultSpeedRatio = 1.0f;
        public static readonly float DefaultEmphasisSpeedRatio = 0.85f;
        public static readonly float DefaultQuoteSpeedRatio = 0.85f;

        public int HeadingAfterPause { get; set; }
        public int ParagraphAfterPause { get; set; }
        public int EmphasisPause { get; set; }
        public int EmphasisPitch { get; set; }
        public float BaseSpeedRatio { get; set; }
        public float EmphasisSpeedRatio { get; set; }
        public float QuoteSpeedRatio { get; set; }

        public NarrationSettings()
        {
            HeadingAfterPause = DefaultHeadingAfterPause;
            ParagraphAfterPause = DefaultParagraphAfterPause;
            EmphasisPause = DefaultEmphasisPause;
            EmphasisPitch = DefaultEmphasisPitch;
            BaseSpeedRatio = DefaultSpeedRatio;
            EmphasisSpeedRatio = DefaultEmphasisSpeedRatio;
            QuoteSpeedRatio = DefaultQuoteSpeedRatio;
        }
    }
}
