using Albite.Reader.Core.Json;
using Albite.Reader.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace Albite.Reader.Speech.Narration
{
    [DataContract]
    public class NarrationSettings
    {
        public static readonly int DefaultHeadingAfterPause = 1000;
        public static readonly int DefaultParagraphAfterPause = 400;
        public static readonly float DefaultSpeedRatio = 1.0f;
        public static readonly float DefaultQuoteSpeedRatio = 0.85f;

        [DataMember]
        public int HeadingAfterPause { get; set; }

        [DataMember]
        public int ParagraphAfterPause { get; set; }

        [DataMember]
        public float BaseSpeedRatio { get; set; }

        [DataMember]
        public float QuoteSpeedRatio { get; set; }

        [DataMember]
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

        public static NarrationSettings FromString(string encodedData)
        {
            if (encodedData == null)
            {
                throw new ArgumentException();
            }

            ISerializer<object> serializer = createSerializer();
            return (NarrationSettings)serializer.Decode(encodedData);
        }

        public override string ToString()
        {
            ISerializer<object> serializer = createSerializer();
            return serializer.Encode(this);
        }

        private static readonly Type[] expectedTypes = new Type[]
        {
            typeof(NarrationSettings),
            typeof(NarrationVoice),
        };

        private static ISerializer<object> createSerializer()
        {
            return new JsonSerializer<object>(expectedTypes);
        }
    }
}
