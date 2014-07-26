namespace Albite.Reader.Speech.Synthesis.Elements
{
    public abstract class LanguageElement : SynthesisElement
    {
        public string Language { get; set; }

        public LanguageElement(string language)
        {
            Language = language;
        }
    }
}
