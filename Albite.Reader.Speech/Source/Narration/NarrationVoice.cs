using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Narration
{
    public class NarrationVoice
    {
        public string Name { get; private set; }

        public string Language { get; private set; }

        public bool Male { get; private set; }

        private NarrationVoice(string name, string language, bool male)
        {
            Name = name;
            Language = language;
            Male = male;
        }

        public NarrationVoice(VoiceInformation v)
            : this(v.DisplayName, v.Language, v.Gender == VoiceGender.Male) { }

        public static NarrationVoice Default
        {
            get
            {
                return new NarrationVoice(InstalledVoices.Default);
            }
        }
    }
}
