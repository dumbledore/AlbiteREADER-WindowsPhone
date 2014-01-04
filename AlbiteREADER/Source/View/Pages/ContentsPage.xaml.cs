using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Core.Collections;
using SvetlinAnkov.Albite.READER.View.Controls;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class ContentsPage : PhoneApplicationPage
    {
        public ContentsPage()
        {
            InitializeComponent();
        }

        private void setCurrentState()
        {
            // Clear the contents
            ContentsList.Children.Clear();

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;

            // Fill the contents
            foreach (INode<IContentItem> node in bookPresenter.Contents)
            {
                // Create the control
                MyControl control = new MyControl(node);

                // Enable tilt effect
                control.SetValue(TiltEffect.IsTiltEnabledProperty, true);

                // Add handler
                control.Tap += control_Tap;
                // Add to the other controls
                ContentsList.Children.Add(control);
            }
        }

        private void control_Tap(object sender, GEArgs e)
        {
            MyControl control = (MyControl)sender;

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

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
                bookPresenter.BookLocation = bookLocation;

                // TODO: Add to history stack
            }

            // Go back to ReaderPage
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

        private class MyControl : HierarchicalTextControl
        {
            public MyControl(INode<IContentItem> node)
            {
                // Set text
                Text = node.Value.Title;

                // Set level
                Level = node.Depth;

                // Set book location
                Location = node.Value.Location;
            }

            public string Location { get; private set; }
        }
    }
}