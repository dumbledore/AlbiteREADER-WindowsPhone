using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using System.Windows.Navigation;
using System.Globalization;
using System;
using Microsoft.Phone.Shell;

namespace Albite.Reader.App.View.Pages
{
    public partial class NarrationPage : PhoneApplicationPage
    {
        private NarrationController narrationController;

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

            // Create a new controller
            narrationController = new NarrationController(Dispatcher, CultureInfo.CurrentCulture.Name);

            // Set up text updates
            narrationController.UpdateText += updateText;

            // And start reading
            narrationController.StartReading();

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

        void updateText(NarrationController sender, string args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                NarrationScroller.ScrollToVerticalOffset(0);
                NarrationBlock.Text = args;
            });
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
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
        }
    }
}