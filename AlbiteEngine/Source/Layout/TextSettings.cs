using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.Engine.Layout
{
    [DataContract]
    public class TextSettings
    {
        [DataMember]
        public LineHeight LineHeight { get; private set; }

        /// <summary>
        /// Whether text is justified or left-aligned.
        /// Default is true (i.e. justified).
        /// </summary>
        [DataMember]
        public bool Justified { get; private set; }

        public TextSettings(LineHeight lineHeight, bool justified)
        {
            LineHeight = lineHeight;
            Justified = justified;
        }
    }
}
