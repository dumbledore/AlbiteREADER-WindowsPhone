using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albite.Reader.Speech.Narration
{
    public class NarrationSettings
    {
        public static readonly int DefaultSentencePause = 200;
        public static readonly int DefaultHeadingSentencePause = 300;
        public static readonly int DefaultHeadingAfterPause = 1500;
        public static readonly int DefaultParagraphAfterPause = 1000;
        public static readonly float DefaultSpeedRatio = 1.0f;
        public static readonly float DefaultEmphasisSpeedRatio = 0.7f;
        public static readonly float DefaultQuoteSpeedRatio = 0.85f;

        public string BaseLanguage { get; set; }
        public int SentencePause { get; set; }
        public int HeadingSentencePause { get; set; }
        public int HeadingAfterPause { get; set; }
        public int ParagraphAfterPause { get; set; }
        public float BaseSpeedRatio { get; set; }
        public float EmphasisSpeedRatio { get; set; }
        public float QuoteSpeedRatio { get; set; }

        public NarrationSettings(string baseLangauge = "en")
        {
            BaseLanguage = baseLangauge;
            SentencePause = DefaultSentencePause;
            HeadingSentencePause = DefaultHeadingSentencePause;
            HeadingAfterPause = DefaultHeadingAfterPause;
            ParagraphAfterPause = DefaultParagraphAfterPause;
            BaseSpeedRatio = DefaultSpeedRatio;
            EmphasisSpeedRatio = DefaultEmphasisSpeedRatio;
            QuoteSpeedRatio = DefaultQuoteSpeedRatio;
        }
    }
}
