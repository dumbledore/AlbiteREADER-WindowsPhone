using Albite.Reader.BookLibrary;
using Albite.Reader.BookLibrary.Location;
using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Core.IO;
using Albite.Reader.Engine.Layout;
using System;

namespace Albite.Reader.Engine.Internal
{
    internal class EngineTemplateController
    {
        // Note: We shan't use Path.Combine for IE would only accept it with "/" slashes
        private static readonly string absoluteContentStylesPath
            = "/" + BookPresenter.RelativeEnginePath + "/" + Paths.ContentStyles;

        private static readonly string tag = "EngineTemplateController";

        public LayoutSettings Settings { get; private set; }

        // We can get the width from any of the templates,
        // so it doesn't matter which one we use.
        public int Width { get { return mainPageTemplate.Width; } }
        public int Height { get { return mainPageTemplate.Height; } }

        private MainPageTemplate mainPageTemplate;
        private BaseStylesTemplate baseStylesTemplate;
        private ContentStylesTemplate contentStylesTemplate;

        public EngineTemplateController(
            LayoutSettings settings,
            string enginePath,
            int initialWidth, int initialHeight,
            int initialApplicationBarHeight)
        {
            Settings = settings;

            // Copy the JSEngine to the Isolated Storage
            using (IsolatedStorage iso = new IsolatedStorage(
                EngineTemplate.GetOutputPath(Paths.JSEngine, enginePath)))
            {
                using (Storage res = EngineTemplate.GetStorage(Paths.JSEngine))
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

            // Set up the layout
            updateLayout();

            // Set up the theme
            updateTheme();
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

        private void updateLayout()
        {
            FontSettings fontSettings = Settings.FontSettings;
            contentStylesTemplate.FontFamily = fontSettings.Family;
            contentStylesTemplate.FontSize = fontSettings.FontSize.Size;

            TextSettings textSettings = Settings.TextSettings;
            contentStylesTemplate.LineHeight = textSettings.LineHeight.Height;
            contentStylesTemplate.Justified = textSettings.Justified;

            contentStylesTemplate.SaveToStorage();
        }

        private void updateTheme()
        {
            string backgroundColorString = Settings.Theme.BackgroundColor.ToHexString();
            string textColorString = Settings.Theme.TextColor.ToHexString();
            string accentColorString = Settings.Theme.AccentColor.ToHexString();

            baseStylesTemplate.BackgroundColor = backgroundColorString;
            baseStylesTemplate.TextColor = textColorString;
            baseStylesTemplate.SaveToStorage();

            contentStylesTemplate.BackgroundColor = backgroundColorString;
            contentStylesTemplate.TextColor = textColorString;
            contentStylesTemplate.AccentColor = accentColorString;
            contentStylesTemplate.SaveToStorage();
        }
    }
}
