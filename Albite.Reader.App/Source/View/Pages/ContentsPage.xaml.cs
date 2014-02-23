using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using Albite.Reader.BookLibrary.Location;
using Albite.Reader.App.View.Controls;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class ContentsPage : PhoneApplicationPage
    {
        public ContentsPage()
        {
            InitializeComponent();

            // Get the context
            Context context = ((IApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;

            // Fill the contents
            ContentsList.ItemsSource = bookPresenter.Contents;
        }

        private void Control_Tap(object sender, GEArgs e)
        {
            TocControl control = (TocControl)sender;

            // Get the context
            Context context = ((IApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Init the path
            string path = control.Location;

            // Init the element id string
            string elementId = null;

            // Is there a hash?
            int hashIndex = path.IndexOf('#');

            if (hashIndex >= 0)
            {
                elementId = path.Substring(hashIndex + 1);
                path = path.Substring(0, hashIndex);
            }

            // Try gettin the chapter for this path
            Chapter chapter = bookPresenter.Spine[path];

            if (chapter != null)
            {
                ChapterLocation chapterLocation;
                if (elementId != null)
                {
                    chapterLocation = new ElementLocation(elementId);
                }
                else
                {
                    chapterLocation = new FirstPageLocation();
                }

                BookLocation bookLocation = chapter.CreateLocation(chapterLocation);

                // Update the reading location
                bookPresenter.HistoryStack.AddNewLocation(bookLocation);
            }

            // Go back to ReaderPage
            NavigationService.GoBack();
        }
    }
}