using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public FontSettings FontSettings { get; set; }

        [DataMember]
        public TextSettings TextSettings { get; set; }

        [DataMember]
        public MarginSettings MarginSettings { get; set; }

        [DataMember]
        public Theme Theme { get; set; }

        public Settings()
        {
            FontSettings = new FontSettings();
            TextSettings = new TextSettings();
            MarginSettings = new MarginSettings();
            Theme = Theme.DefaultTheme;
        }

        public static Settings FromString(string encodedData)
        {
            if (encodedData == null)
            {
                throw new ArgumentException();
            }

            SettingsSerializer serializer = new SettingsSerializer();
            return (Settings)serializer.Decode(encodedData);
        }

        public override string ToString()
        {
            SettingsSerializer serializer = new SettingsSerializer();
            return serializer.Encode(this);
        }
    }
}
