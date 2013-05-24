using System;
using System.Net;
using System.Windows;
using System.Reflection;
using SvetlinAnkov.Albite.Core.Utils;
using System.IO;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public class EngineTemplate : TemplateResource
    {
        public EngineTemplate(string path, string enginePath)
            : base(getTemplate(path), GetOutputPath(path, enginePath))
        {
        }

        private static string assemblyName;
        private static string getAssemblyName()
        {
            if (assemblyName == null)
            {
                AssemblyName name = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                assemblyName = name.Name;
            }
            return assemblyName;
        }

        private static string getTemplate(string path)
        {
            using (AlbiteStorage res = GetStorage(path))
            {
                return res.ReadAsString();
            }
        }

        public static AlbiteStorage GetStorage(string path)
        {
            string inputFilename = Path.Combine(Paths.BasePath, path);
            return new AlbiteResourceStorage(inputFilename, getAssemblyName());
        }

        public static string GetOutputPath(string path, string enginePath)
        {
            return Path.Combine(enginePath, path);
        }

        protected void set(string name, string value)
        {
            this[name] = value;
        }

        protected void set(string name, object value)
        {
            this[name] = value.ToString();
        }
    }

    public class MainPageTemplate : EngineTemplate
    {
        public MainPageTemplate(string enginePath)
            : base(Paths.MainPage, enginePath)
        { }

        private bool debug = false;
        public bool Debug
        {
            get { return debug; }
            set
            {
                set("debug", value ? "true" : "false");
                debug = value;
            }
        }

        private int fullPageWidth;
        public int FullPageWidth
        {
            get { return fullPageWidth; }
            set
            {
                set("full_page_width", value);
                fullPageWidth = value;
            }
        }

        private int viewportWidth;
        public int ViewportWidth
        {
            get { return viewportWidth; }
            set
            {
                set("viewport_width", value);
                viewportWidth = value;
            }
        }

        private string initialLocation;
        public string InitialLocation
        {
            get { return initialLocation; }
            set
            {
                set("initial_location", value);
                initialLocation = value;
            }
        }

        private string chapterFile;
        public string ChatperFile
        {
            get { return chapterFile; }
            set
            {
                set("chapter_file", value);
                chapterFile = value;
            }
        }
    }

    public class BaseStylesTemplate : EngineTemplate
    {
        public BaseStylesTemplate(string enginePath)
            : base(Paths.BaseStyles, enginePath)
        { }

        // Colors
        private string controlBackground;
        public string ControlBackground
        {
            get { return controlBackground; }
            set
            {
                set("control_background", value);
                controlBackground = value;
            }
        }

        private string backgroundColor;
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                set("background_color", value);
                backgroundColor = value;
            }
        }

        private string textColor;
        public string TextColor
        {
            get { return textColor; }
            set
            {
                set("text_color", value);
                textColor = value;
            }
        }

        // Margins
        private int marginTop;
        public int MarginTop
        {
            get { return marginTop; }
            set
            {
                set("page_margin_top", value);
                marginTop = value;
            }
        }

        private int marginBottom;
        public int MarginBottom
        {
            get { return marginBottom; }
            set
            {
                set("page_margin_bottom", value);
                marginBottom = value;
            }
        }

        private int marginLeft;
        public int MarginLeft
        {
            get { return marginLeft; }
            set
            {
                set("page_margin_left", value);
                marginLeft = value;
            }
        }

        private int marginRight;
        public int MarginRight
        {
            get { return marginRight; }
            set
            {
                set("page_margin_right", value);
                marginRight = value;
            }
        }

        // Page Dimensions
        private int pageWidth;
        public int PageWidth
        {
            get { return pageWidth; }
            set
            {
                set("page_width", value);
                pageWidth = value;
            }
        }

        private int pageHeight;
        public int PageHeight
        {
            get { return pageHeight; }
            set
            {
                set("page_height", value);
                pageHeight = value;
            }
        }

        private int viewportWidth;
        public int ViewportWidth
        {
            get { return viewportWidth; }
            set
            {
                set("viewport_width", value);
                viewportWidth = value;
            }
        }
    }

    public class ContentStylesTemplate : EngineTemplate
    {
        public ContentStylesTemplate(string enginePath)
            : base(Paths.ContentStyles, enginePath)
        { }

        // Colors
        private string backgroundColor;
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                set("background_color", value);
                backgroundColor = value;
            }
        }

        private string textColor;
        public string TextColor
        {
            get { return textColor; }
            set
            {
                set("text_color", value);
                textColor = value;
            }
        }

        private string accentColor;
        public string AccentColor
        {
            get { return accentColor; }
            set
            {
                set("accent_color", value);
                accentColor = value;
            }
        }

        // Text Layout
        private int lineHeight;
        public int LineHeight
        {
            get { return lineHeight; }
            set
            {
                set("line_height", value);
                lineHeight = value;
            }
        }

        private int fontSize;
        public int FontSize
        {
            get { return fontSize; }
            set
            {
                set("font_size", value);
                fontSize = value;
            }
        }

        private string fontFamily;
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                set("font_family", value);
                fontFamily = value;
            }
        }

        private TextAlign textAligh;
        public TextAlign TextAlign
        {
            get { return textAligh; }
            set
            {
                set("text_align", value);
                textAligh = value;
            }
        }
    }
}
