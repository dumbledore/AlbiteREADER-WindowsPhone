using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    [DataContract]
    public class FontSettings
    {
        /// <summary>
        /// CSS font family. Default is Georgia.
        /// </summary>
        [DataMember]
        public string Family { get; set; }

        /// <summary>
        /// CSS font size in pixels. Default is 28.
        /// </summary>
        [DataMember]
        public int Size { get; set; }

        public FontSettings()
        {
            Family = "Georgia";
            Size = 28;
        }
    }
}
