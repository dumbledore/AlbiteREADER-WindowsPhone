using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.READER.View.Transition;
using System;
using System.Windows;
using System.Windows.Navigation;
using AlbiteTransitionFrame = SvetlinAnkov.Albite.READER.View.Transition.TransitionFrame;

namespace SvetlinAnkov.Albite.READER
{
    public partial class App : Application, IAlbiteApplication
    {
        public static string Tag { get { return "App"; } }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        private AlbiteContext context = new AlbiteContext("Library/", "Albite.sdf");
        public AlbiteContext CurrentContext { get { return context; } }

        /// <summary>
        /// Helper function that returns the AlbiteContext
        /// for the running application.
        /// </summary>
        public static AlbiteContext Context
        {
            get
            {
                return ((IAlbiteApplication) Current).CurrentContext;
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

#if false
            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
#endif
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Log.Flush();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Log.E(Tag, "Navigation Failed: " + e.Uri, e.Exception);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Log.E(Tag, "Unhandled Exception", e.ExceptionObject);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = createTransitionFrame();

            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Custom uri mapper
            RootFrame.UriMapper = new AlbiteUriMapper();

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        private PhoneApplicationFrame createTransitionFrame()
        {
            INavigationTransitionFactory navigationTransitionFactory
                = new DramaticTransition.Factory(new Duration(TimeSpan.FromMilliseconds(400)), 1.2, 0.8);

            IRotationTransitionFactory rotationTransitionFactory
                = new RotationTransition.Factory(new Duration(TimeSpan.FromMilliseconds(300)));

            return new AlbiteTransitionFrame(navigationTransitionFactory, rotationTransitionFactory);
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}