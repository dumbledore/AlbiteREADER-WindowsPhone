using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Core.Utils;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    internal class EngineTemplate : TemplateResource
    {
        private readonly string[] updateTriggers;

        public EngineTemplate(string path, string enginePath, string[] updateTriggers)
            : base(getTemplate(path), GetOutputPath(path, enginePath))
        {
            this.updateTriggers = updateTriggers;
        }

        public EngineTemplate(string path, string enginePath)
            : this(path, enginePath, null)
        { }

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
            checkDepencies(name);
        }

        protected void set(string name, object value)
        {
            set(name, value.ToString());
        }

        // Boolean.ToString() returns True or False,
        // i.e. with capital letters. That's not OK
        // with Javascript
        protected void set(string name, bool value)
        {
            set(name, value ? "true" : "false");
        }

        private void checkDepencies(string name)
        {
            if (updateTriggers != null && updateTriggers.Contains(name))
            {
                UpdateDependencies();
            }
        }

        // Called when setting values
        protected virtual void UpdateDependencies() { }

        protected static class RegisteredNames
        {
            public static readonly string DebugEnabled = "debug_enabled";
            public static readonly string InitialLocation = "initial_location";
            public static readonly string IsFirstChapter = "is_first_chapter";
            public static readonly string IsLastChapter = "is_last_chapter";
            public static readonly string ChapterFile = "chapter_file";

            public static readonly string Width = "width";
            public static readonly string Height = "height";
            public static readonly string MarginTop = "margin_top";
            public static readonly string MarginBottom = "margin_bottom";
            public static readonly string MarginLeft = "margin_left";
            public static readonly string MarginRight = "margin_right";

            public static readonly string ContentWidth = "content_width";
            public static readonly string ContentHeight = "content_height";

            public static readonly string BackgroundColor = "background_color";
            public static readonly string TextColor = "text_color";
            public static readonly string AccentColor = "accent_color";

            public static readonly string LineHeight = "line_height";
            public static readonly string FontSize = "font_size";
            public static readonly string FontFamily = "font_family";
            public static readonly string TextAlign = "text_align";
        }
    }

    internal class MainPageTemplate : EngineTemplate
    {
        public MainPageTemplate(string enginePath)
            : base(Paths.MainPage, enginePath)
        { }

        private bool debugEnabled = false;
        public bool DebugEnabled
        {
            get { return debugEnabled; }
            set
            {
                // Note: Always update the value first as set() might
                // call UpdateDependencies() which might need to use it,
                // otherwise they'll use a stale value.
                debugEnabled = value;
                set(RegisteredNames.DebugEnabled, value ? "true" : "false");
            }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                set(RegisteredNames.Width, value);
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                set(RegisteredNames.Height, value);
            }
        }

        private string initialLocation;
        public string InitialLocation
        {
            get { return initialLocation; }
            set
            {
                initialLocation = value;
                set(RegisteredNames.InitialLocation, value);
            }
        }

        private bool isFirstChapter;
        public bool IsFirstChapter
        {
            get { return isFirstChapter; }
            set
            {
                isFirstChapter = value;
                set(RegisteredNames.IsFirstChapter, value);
            }
        }

        private bool isLastChapter;
        public bool IsLastChapter
        {
            get { return isLastChapter; }
            set
            {
                isLastChapter = value;
                set(RegisteredNames.IsLastChapter, value);
            }
        }

        private string chapterFile;
        public string ChatperFile
        {
            get { return chapterFile; }
            set
            {
                chapterFile = value;
                set(RegisteredNames.ChapterFile, value);
            }
        }
    }

    internal class BaseStylesTemplate : EngineTemplate
    {
        public BaseStylesTemplate(string enginePath)
            : base(Paths.BaseStyles, enginePath)
        { }

        // Colors
        private string backgroundColor;
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                set(RegisteredNames.BackgroundColor, value);
            }
        }

        private string textColor;
        public string TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                set(RegisteredNames.TextColor, value);
            }
        }

        // Page Dimensions
        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                set(RegisteredNames.Width, value);
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                set(RegisteredNames.Height, value);
            }
        }
    }

    internal class ContentStylesTemplate : EngineTemplate
    {
        private static readonly string[] triggers = {
            RegisteredNames.Width,
            RegisteredNames.Height,
            RegisteredNames.MarginTop,
            RegisteredNames.MarginBottom,
            RegisteredNames.MarginLeft,
            RegisteredNames.MarginRight,
        };

        public ContentStylesTemplate(string enginePath)
            : base(Paths.ContentStyles, enginePath, triggers)
        { }

        // Colors
        private string backgroundColor;
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                set(RegisteredNames.BackgroundColor, value);
            }
        }

        private string textColor;
        public string TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                set(RegisteredNames.TextColor, value);
            }
        }

        private string accentColor;
        public string AccentColor
        {
            get { return accentColor; }
            set
            {
                accentColor = value;
                set(RegisteredNames.AccentColor, value);
            }
        }

        // Text Layout
        private int lineHeight;
        public int LineHeight
        {
            get { return lineHeight; }
            set
            {
                lineHeight = value;
                set(RegisteredNames.LineHeight, value);
            }
        }

        private int fontSize;
        public int FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                set(RegisteredNames.FontSize, value);
            }
        }

        private string fontFamily;
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                set(RegisteredNames.FontFamily, value);
            }
        }

        private TextAlign textAligh;
        public TextAlign TextAlign
        {
            get { return textAligh; }
            set
            {
                textAligh = value;
                set(RegisteredNames.TextAlign, value);
            }
        }

        // Margins
        private int marginTop;
        public int MarginTop
        {
            get { return marginTop; }
            set
            {
                marginTop = value;
                set(RegisteredNames.MarginTop, value);
            }
        }

        private int marginBottom;
        public int MarginBottom
        {
            get { return marginBottom; }
            set
            {
                marginBottom = value;
                set(RegisteredNames.MarginBottom, value);
            }
        }

        private int marginLeft;
        public int MarginLeft
        {
            get { return marginLeft; }
            set
            {
                marginLeft = value;
                set(RegisteredNames.MarginLeft, value);
            }
        }

        private int marginRight;
        public int MarginRight
        {
            get { return marginRight; }
            set
            {
                marginRight = value;
                set(RegisteredNames.MarginRight, value);
            }
        }

        // Page Dimensions
        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                set(RegisteredNames.Width, value);
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;

                //It's not really there, but it's the contentType
                // depends on it, so emulate it.
                UpdateDependencies();
            }
        }

        protected override void UpdateDependencies()
        {
            int contentHeight = Height - (MarginTop + MarginBottom);
            set(RegisteredNames.ContentHeight, contentHeight);
        }
    }
}
