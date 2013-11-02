namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class TextSettings
    {
        /// <summary>
        /// Line height in percent. Default is 140%.
        /// </summary>
        public int LineHeight { get; set; }

        /// <summary>
        /// Whether text is justified or left-aligned.
        /// Default is true (i.e. justified).
        /// </summary>
        public bool Justified { get; set; }

        public TextSettings()
        {
            LineHeight = 140;
            Justified = true;
        }
    }
}
