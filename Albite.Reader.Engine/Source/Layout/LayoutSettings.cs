using Albite.Reader.Core.Json;
using Albite.Reader.Core.Serialization;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Albite.Reader.Engine.Layout
{
    [DataContract]
    public class LayoutSettings
    {
        [DataMember]
        public FontSettings FontSettings { get; private set; }

        [DataMember]
        public TextSettings TextSettings { get; private set; }

        [DataMember]
        public MarginSettings MarginSettings { get; private set; }

        [DataMember]
        public Theme Theme { get; private set; }

        public LayoutSettings(
            FontSettings fontSettings,
            TextSettings textSettings,
            MarginSettings marginSettings,
            Theme theme)
        {
            FontSettings = fontSettings;
            TextSettings = textSettings;
            MarginSettings = marginSettings;
            Theme = theme;
        }

        public static LayoutSettings FromString(string encodedData)
        {
            if (encodedData == null)
            {
                throw new ArgumentException();
            }

            ISerializer<object> serializer = createSerializer();
            return (LayoutSettings)serializer.Decode(encodedData);
        }

        public override string ToString()
        {
            ISerializer<object> serializer = createSerializer();
            return serializer.Encode(this);
        }

        private static readonly Type[] expectedTypes = new Type[]
        {
            typeof(FontSettings),
            typeof(FontSize),
            typeof(LayoutSettings),
            typeof(LineHeight),
            typeof(MarginSettings),
            typeof(TextSettings),
            typeof(Theme),
            typeof(Color),
        };

        private static ISerializer<object> createSerializer()
        {
            return new JsonSerializer<object>(expectedTypes);
        }
    }
}
