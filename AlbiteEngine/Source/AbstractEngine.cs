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

        private int actualWidth = 0;
        private int actualHeight = 0;

        public AbstractEngine(
            IEngineController controller, BookPresenter bookPresenter, Settings settings)
        {
            Controller = controller;
            BookPresenter = bookPresenter;
            Settings = settings;

            // Set up the base path
            Controller.BasePath = BookPresenter.Path;

            Uri = new Uri(
                Path.Combine(BookPresenter.RelativeEnginePath, Paths.MainPage),
                UriKind.Relative);

            TemplateController = getTemplateController();

            AbstractNavigator = GetNavigator();
            Messenger = getMessenger();

            // Don't forget to update the cached dimensions
            actualWidth = controller.Width;
            actualHeight = controller.Height;
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
            int width = Controller.Width;
            int height = Controller.Height;

            if (Controller.IsLoading)
            {
                Log.D(tag, "Can't update the dimensions while loading");
                return;
            }

            if (width == actualWidth
                && height == actualHeight)
            {
                Log.D(tag, "The dimensions haven't changed, no need to update.");
                return;
            }

            // Update the cached values
            actualWidth = width;
            actualHeight = height;

            // Update the templates
            TemplateController.UpdateDimensions(width, height);

            // Reload to the current DomLocation
            AbstractNavigator.Reload();
        }

        public void ReceiveMessage(string message)
        {
            Messenger.NotifyHost(message);
        }
    }
}
