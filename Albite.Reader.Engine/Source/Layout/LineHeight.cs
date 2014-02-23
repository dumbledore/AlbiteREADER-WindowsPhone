using System.Runtime.Serialization;

namespace Albite.Reader.Engine.Layout
{
    [DataContract]
    public class LineHeight
    {
        /// <summary>
        /// Name of the current line height set-up,
        /// e.g. Loose, Tight, etc.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Line height in percent. Default is 140%.
        /// </summary>
        [DataMember]
        public int Height { get; private set; }

        public LineHeight(string name, int height)
        {
            Name = name;
            Height = height;
        }
    }
}
