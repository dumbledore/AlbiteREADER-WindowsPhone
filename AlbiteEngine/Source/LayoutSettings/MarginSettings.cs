namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class MarginSettings
    {
        /// <summary>
        /// Size of the top margin in percents of the longer side.
        /// Default is 4%.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Size of the bottom margin in percents of the longer side.
        /// Default is 4%.
        /// </summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Size of the left margin in percents of the longer side.
        /// Default is 4%.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Size of the right margin in percents of the longer side.
        /// Default is 4%.
        /// </summary>
        public int Right { get; set; }

        public MarginSettings()
        {
            Top = 4;
            Bottom = 4;
            Left = 4;
            Right = 4;
        }
    }
}
