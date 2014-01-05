using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using System;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class ReadingLocationPage : PhoneApplicationPage
    {
        public ReadingLocationPage()
        {
            InitializeComponent();
        }

        private const int stepsPerChapter = 10;

        private int initialLocation;

        private void setCurrentState()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;

            // Get the current location
            BookLocation location = bookPresenter.HistoryStack.GetCurrentLocation();

            // Update slider's maximum number
            // We need to subtract 1, because we start at 0, e.g.
            // Chapter 0 is  0, ...,  9
            // Chapter 1 is 10, ..., 19
            LocationSlider.Maximum = (bookPresenter.Spine.Length * stepsPerChapter) - 1;

            // Calculate chapter position
            int currentPosition = location.Chapter.Number * stepsPerChapter;

            // Now add the % in the chapter
            // We need to subtract 1, because the chapter is in the range xxx0, ..., xxx9, e.g.
            int chapterOffset = (int)(location.Location.RelativeLocation * (stepsPerChapter - 1));

            // Ready to set the position
            initialLocation = currentPosition + chapterOffset;
            LocationSlider.Value = initialLocation;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

        private void applyLocation()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Get new location
            int currentLocation = (int)LocationSlider.Value;

            if (initialLocation != currentLocation)
            {
                // Get the chapter number
                int chapterNumber = currentLocation / stepsPerChapter;

                // Get the offset
                double chapterOffset = (currentLocation % stepsPerChapter) / ((double) (stepsPerChapter - 1));

                // Get the chapter
                Chapter chapter = bookPresenter.Spine[chapterNumber];

                // Create the RelativeLocation
                RelativeChapterLocation relativeLocation = new RelativeChapterLocation(chapterOffset);

                // Create the BookLocation
                BookLocation location = chapter.CreateLocation(relativeLocation);

                // Set the new location
                bookPresenter.HistoryStack.AddNewLocation(location);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // First apply the new settings
            applyLocation();

            // Go on as usual
            base.OnNavigatingFrom(e);
        }

        private void GoToButton_Click(object sender, EventArgs e)
        {
            // Simply go back to the ReaderPage
            NavigationService.GoBack();
        }
    }
}