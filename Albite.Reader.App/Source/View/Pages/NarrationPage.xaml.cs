using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using System.Windows.Navigation;
using System.Globalization;

namespace Albite.Reader.App.View.Pages
{
    public partial class NarrationPage : PhoneApplicationPage
    {
        private NarrationController narrationController;

        public NarrationPage()
        {
            InitializeComponent();

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
                NarrationBlock.Text = args;
            });
        }
    }
}