﻿using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using System.Windows.Navigation;
using System;
using Microsoft.Phone.Shell;
using Albite.Reader.Speech.Narration;

namespace Albite.Reader.App.View.Pages.Narration
{
    public partial class NarrationPage : PhoneApplicationPage
    {
        private NarrationController narrationController;
        private bool reading = false;

        public NarrationPage()
        {
            InitializeComponent();

            // Get the bar buttons ready
            initializeApplicationBar();

            // Get the book presenter
            BookPresenter bookPresenter = App.Context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                int bookId = int.Parse(NavigationContext.QueryString["id"]);
                App.Context.OpenBook(bookId);
            }

            // Get current narration settings
            NarrationSettings settings = App.Context.NarrationSettings;

            // Create a new controller
            narrationController = new NarrationController(settings, Dispatcher);

            // Set up text updates
            narrationController.UpdateText += updateText;

            // Set end of book events
            narrationController.NarrationEnded += narrationEndedDelegate;

            // And start reading
            narrationController.StartReading();

            // Update PlayButton to "Pause"
            updateReadingState(true);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Persist the current position
            narrationController.StopReadingAndPersist();

            // Free resources
            narrationController.Dispose();

            // We don't need the controller anymore
            narrationController = null;

            base.OnNavigatedFrom(e);
        }

        private void updateText(NarrationController sender, string args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                NarrationScroller.ScrollToVerticalOffset(0);
                NarrationBlock.Text = args;
            });
        }

        private static readonly Uri PlayUri = new Uri("/Resources/Images/transport.play.png", UriKind.Relative);
        private static readonly Uri PauseUri = new Uri("/Resources/Images/transport.pause.png", UriKind.Relative);

        private bool updateReadingState(bool reading)
        {
            // Cache the old state
            bool wasReading = this.reading;

            // Save the new state
            this.reading = reading;

            // And update the button
            PlayButton.IconUri = reading ? PauseUri : PlayUri;
            PlayButton.Text = reading ? "pause" : "play";

            // Return the old state
            return wasReading;
        }

        void narrationEndedDelegate(object sender, EventArgs e)
        {
            // Called from the main thread, so there's no need for a lock!
            // That's not necessarily obvious, but it's because of how
            // NarrationController works, i.e. it would called on the main thread
            updateReadingState(false);
        }

        private void initializeApplicationBar()
        {
            // The buttons can't be addressed using "x:Name"
            PreviousButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            PlayButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            NextButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
            SettingsButton = ApplicationBar.Buttons[3] as ApplicationBarIconButton;
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            // Stop reading
            narrationController.StopReading();

            // Go back
            narrationController.GoBack();

            // Restore reading state
            if (reading)
            {
                narrationController.StartReading();
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (updateReadingState(!reading))
            {
                narrationController.StopReading();
            }
            else
            {
                narrationController.StartReading();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            // Stop reading
            narrationController.StopReading();

            // Go forward
            narrationController.GoForward();

            // Restore reading state
            if (reading)
            {
                narrationController.StartReading();
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/Narration/NarrationSettingsPage.xaml", UriKind.Relative));
        }
    }
}