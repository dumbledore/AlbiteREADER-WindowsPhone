namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class FontSettings
    {
        /// <summary>
        /// CSS font family. Default is Georgia.
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// CSS font size in pixels. Default is 28.
        /// </summary>
        public int Size { get; set; }

        public FontSettings()
        {
            Family = "Georgia";
            Size = 28;
        }
    }
}
