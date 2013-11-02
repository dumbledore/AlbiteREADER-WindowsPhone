using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Engine.LayoutSettings;
using System;
using System.IO;

namespace SvetlinAnkov.Albite.Engine
{
    public abstract class AbstractEngine : IEngine
    {
        private static readonly string tag = "AbstractEngine";

        public BookPresenter BookPresenter { get; private set; }
        public Settings Settings { get; private set; }

        protected AbstractNavigator AbstractNavigator;
        public IEngineNavigator Navigator { get { return AbstractNavigator; } }
        public Uri Uri { get; private set; }

        internal readonly IEngineController Controller;
        internal readonly EngineTemplateController TemplateController;
        internal readonly AlbiteMessenger Messenger;

        public AbstractEngine(
            IEngineController controller, BookPresenter bookPresenter, Settings settings)
        {
            Controller = controller;
            BookPresenter = bookPresenter;
            Settings = settings;

            // Set up the base path
            Controller.BasePath = BookPresenter.Path;

            // Uri of main.xhtml
            Uri = new Uri(
                Path.Combine(BookPresenter.RelativeEnginePath, Paths.MainPage),
                UriKind.Relative);

            TemplateController = getTemplateController();
            AbstractNavigator = GetNavigator();
            Messenger = getMessenger();
        }

        protected abstract AbstractNavigator GetNavigator();

        private AlbiteMessenger getMessenger()
        {
            return new AlbiteMessenger(
                new ClientMessenger(Controller),
                new HostMessenger(this)
            );
        }

        private EngineTemplateController getTemplateController()
        {
            return new EngineTemplateController(
                Settings, BookPresenter.EnginePath,
                Controller.Width, Controller.Height);
        }

        public void UpdateLayout()
        {
            // Update the templates
            TemplateController.UpdateSettings();

            // Reload to the current DomLocation
            AbstractNavigator.Reload();
        }

        public void UpdateDimensions()
        {
            // Get current dimensions
            int width = TemplateController.Width;
            int height = TemplateController.Height;

            // New dimensions
            int newWidth = Controller.Width;
            int newHeight = Controller.Height;

            if (Controller.IsLoading)
            {
                Log.D(tag, "Can't update the dimensions while loading");
                return;
            }

            if (newWidth == width
                && newHeight == height)
            {
                Log.D(tag, "The dimensions haven't changed, no need to update.");
                return;
            }

            // Update the templates
            TemplateController.UpdateDimensions(newWidth, newHeight);

            // Reload to the current DomLocation
            AbstractNavigator.Reload();
        }

        public void ReceiveMessage(string message)
        {
            Messenger.NotifyHost(message);
        }
    }
}
