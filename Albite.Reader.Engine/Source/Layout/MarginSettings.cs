using System.Runtime.Serialization;

namespace Albite.Reader.Engine.Layout
{
    [DataContract]
    public class MarginSettings
    {
        /// <summary>
        /// Name for this margin set-up,
        /// e.g. Narrow, Wide, etc.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Size of the top margin in percents of the longer side
        /// </summary>
        [DataMember]
        public int Top { get; private set; }

        /// <summary>
        /// Size of the bottom margin in percents of the longer side
        /// </summary>
        [DataMember]
        public int Bottom { get; private set; }

        /// <summary>
        /// Size of the left margin in percents of the longer side
        /// </summary>
        [DataMember]
        public int Left { get; private set; }

        /// <summary>
        /// Size of the right margin in percents of the longer side
        /// </summary>
        [DataMember]
        public int Right { get; private set; }

        public MarginSettings(
            string name,
            int left, int top, int right, int bottom)
        {
            Name = name;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public MarginSettings(string name, int margin)
            : this(name, margin, margin, margin, margin) { }
    }
}
