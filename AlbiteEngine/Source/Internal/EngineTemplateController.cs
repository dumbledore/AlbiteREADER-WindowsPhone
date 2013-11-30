using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Engine.LayoutSettings;
using System;

namespace SvetlinAnkov.Albite.Engine.Internal
{
    internal class EngineTemplateController
    {
        // Note: We shan't use Path.Combine for IE would only accept it with "/" slashes
        private static readonly string absoluteContentStylesPath
            = "/" + BookPresenter.RelativeEnginePath + "/" + Paths.ContentStyles;

        private static readonly string tag = "EngineTemplateController";

        public Settings Settings { get; private set; }

        // We can get the width from any of the templates,
        // so it doesn't matter which one we use.
        public int Width { get { return mainPageTemplate.Width; } }
        public int Height { get { return mainPageTemplate.Height; } }

        private MainPageTemplate mainPageTemplate;
        private BaseStylesTemplate baseStylesTemplate;
        private ContentStylesTemplate contentStylesTemplate;

        public EngineTemplateController(Settings settings,
            string enginePath, int initialWidth, int initialHeight,
            int initialApplicationBarHeight)
        {
            Settings = settings;

            // Copy the JSEngine to the Isolated Storage
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(
                EngineTemplate.GetOutputPath(Paths.JSEngine, enginePath)))
            {
                using (AlbiteStorage res = EngineTemplate.GetStorage(Paths.JSEngine))
                {
                    res.CopyTo(iso);
                }
            }

            // Load the templates
            mainPageTemplate = new MainPageTemplate(enginePath);
#if DEBUG
            mainPageTemplate.DebugEnabled = true;
#else
            mainPageTemplate.DebugEnabled = false;
#endif
            baseStylesTemplate = new BaseStylesTemplate(enginePath);
            contentStylesTemplate = new ContentStylesTemplate(enginePath);

            // Set up css location
            mainPageTemplate.CssLocation = absoluteContentStylesPath;

            // Set up the namespace
            mainPageTemplate.TypeNamespace = EngineMessenger.TypeNamespace;

            // Set up the dimensions
            UpdateDimensions(initialWidth, initialHeight, initialApplicationBarHeight);

            // Set up the settings
            UpdateSettings();
        }

        public void UpdateInitialLocation(
            ChapterLocation initialLocation)
        {
            mainPageTemplate.InitialLocation = initialLocation;
            mainPageTemplate.SaveToStorage();
        }

        public void UpdateChapter(
            ChapterLocation initialLocation,
            bool isFirstChapter, bool isLastChapter,
            string filePath)
        {
            mainPageTemplate.InitialLocation = initialLocation;
            mainPageTemplate.IsFirstChapter = isFirstChapter;
            mainPageTemplate.IsLastChapter = isLastChapter;
            mainPageTemplate.ChatperFile = filePath;
            mainPageTemplate.SaveToStorage();
        }

        public void UpdateDimensions(int width, int height, int applicationBarHeight)
        {
            Log.D(tag, string.Format(
                "UpdateDimensions: {0}x{1}", width, height));

            mainPageTemplate.Width = width;
            mainPageTemplate.Height = height;
            mainPageTemplate.SaveToStorage();

            baseStylesTemplate.Width = width;
            baseStylesTemplate.Height = height;
            baseStylesTemplate.SaveToStorage();

            int viewportReference = Math.Max(width, width);
            MarginSettings margins = Settings.MarginSettings;
            int marginLeft = (margins.Left * viewportReference) / 100;
            int marginRight = (margins.Right * viewportReference) / 100;
            int marginTop = (margins.Top * viewportReference) / 100;
            int marginBottom = (margins.Bottom * viewportReference) / 100;

            contentStylesTemplate.MarginTop = marginTop;
            contentStylesTemplate.MarginBottom = marginBottom + applicationBarHeight;
            contentStylesTemplate.MarginLeft = marginLeft;
            contentStylesTemplate.MarginRight = marginRight;

            contentStylesTemplate.Width = width;
            contentStylesTemplate.Height = height;

            contentStylesTemplate.SaveToStorage();
        }

        public void UpdateSettings()
        {
            updateLayout();
            updateTheme();
        }

        private void updateLayout()
        {
            FontSettings fontSettings = Settings.FontSettings;
            contentStylesTemplate.FontFamily = fontSettings.Family;
            contentStylesTemplate.FontSize = fontSettings.Size;

            TextSettings textSettings = Settings.TextSettings;
            contentStylesTemplate.LineHeight = textSettings.LineHeight;
            contentStylesTemplate.Justified = textSettings.Justified;

            contentStylesTemplate.SaveToStorage();
        }

        private void updateTheme()
        {
            baseStylesTemplate.BackgroundColor = Settings.Theme.BackgroundColor.HtmlColor;
            baseStylesTemplate.TextColor = Settings.Theme.TextColor.HtmlColor;
            baseStylesTemplate.SaveToStorage();

            contentStylesTemplate.BackgroundColor = Settings.Theme.BackgroundColor.HtmlColor;
            contentStylesTemplate.TextColor = Settings.Theme.TextColor.HtmlColor;
            contentStylesTemplate.AccentColor = Settings.Theme.AccentColor.HtmlColor;
            contentStylesTemplate.SaveToStorage();
        }
    }
}
