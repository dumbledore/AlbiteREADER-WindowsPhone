using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

            SettingsSerializer serializer = new SettingsSerializer();
            return (LayoutSettings)serializer.Decode(encodedData);
        }

        public override string ToString()
        {
            SettingsSerializer serializer = new SettingsSerializer();
            return serializer.Encode(this);
        }
    }
}
