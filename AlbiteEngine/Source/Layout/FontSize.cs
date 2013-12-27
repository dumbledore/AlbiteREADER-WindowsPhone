using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.Engine.Layout
{
    [DataContract]
    public class FontSize
    {
        /// <summary>
        /// Name for this size set-up,
        /// e.g. Small, Large, etc.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// CSS font size in pixels
        /// </summary>
        [DataMember]
        public int Size { get; private set; }

        public FontSize(string name, int pixelSize)
        {
            Name = name;
            Size = pixelSize;
        }
    }
}
