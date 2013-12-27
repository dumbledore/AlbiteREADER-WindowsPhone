using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.Engine.Layout
{
    [DataContract]
    public class FontSettings
    {
        /// <summary>
        /// CSS font family
        /// </summary>
        [DataMember]
        public string Family { get; private set; }

        /// <summary>
        /// CSS font size
        /// </summary>
        [DataMember]
        public FontSize FontSize { get; private set; }

        public FontSettings(string family, FontSize fontSize)
        {
            Family = family;
            FontSize = fontSize;
        }
    }
}
