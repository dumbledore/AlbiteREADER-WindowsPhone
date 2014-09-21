namespace Albite.Reader.Speech.Narration
{
    public class NarrationSettings
    {
        public static readonly int DefaultHeadingAfterPause = 1000;
        public static readonly int DefaultParagraphAfterPause = 400;
        public static readonly float DefaultSpeedRatio = 1.0f;
        public static readonly float DefaultQuoteSpeedRatio = 0.85f;

        public int HeadingAfterPause { get; set; }
        public int ParagraphAfterPause { get; set; }
        public float BaseSpeedRatio { get; set; }
        public float QuoteSpeedRatio { get; set; }
        public NarrationVoice BaseVoice { get; set; }

        public NarrationSettings(NarrationVoice baseVoice)
        {
            HeadingAfterPause = DefaultHeadingAfterPause;
            ParagraphAfterPause = DefaultParagraphAfterPause;
            BaseSpeedRatio = DefaultSpeedRatio;
            QuoteSpeedRatio = DefaultQuoteSpeedRatio;
            BaseVoice = baseVoice;
        }

        public NarrationSettings() : this(NarrationVoice.Default) { }
    }
}
