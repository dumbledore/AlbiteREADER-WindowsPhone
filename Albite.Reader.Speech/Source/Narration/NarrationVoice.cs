using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Narration
{
    public class NarrationVoice
    {
        public string Language { get; private set; }

        public bool Male { get; private set; }

        public NarrationVoice(string language, bool male)
        {
            Language = language;
            Male = male;
        }

        public static NarrationVoice Default
        {
            get
            {
                VoiceInformation v = InstalledVoices.Default;
                return new NarrationVoice(v.Language, v.Gender == VoiceGender.Male);
            }
        }
    }
}
