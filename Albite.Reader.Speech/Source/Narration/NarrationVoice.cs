using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Narration
{
    [DataContract]
    public class NarrationVoice
    {
        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Language { get; private set; }

        [DataMember]
        public bool Male { get; private set; }

        private NarrationVoice(string name, string language, bool male)
        {
            Name = name;
            Language = language;
            Male = male;
        }

        private NarrationVoice(VoiceInformation v)
            : this(v.DisplayName, v.Language, v.Gender == VoiceGender.Male) { }

        public static NarrationVoice Default
        {
            get
            {
                return new NarrationVoice(InstalledVoices.Default);
            }
        }

        public static IEnumerable<NarrationVoice> Voices
        {
            get
            {
                // Get the voices using linq. Using var here is necessary
                var voices = from v in InstalledVoices.All select new NarrationVoice(v);

                // And done
                return voices;
            }
        }
    }
}
