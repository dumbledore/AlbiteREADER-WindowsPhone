using System.Windows;
using System.Windows.Controls;
using Windows.Foundation;
using System.Windows.Input;

namespace Albite.Reader.App.View.Controls
{
    public partial class VoiceSearchControl : UserControl
    {
        public VoiceSearchControl()
        {
            InitializeComponent();
        }

        public event TypedEventHandler<VoiceSearchControl, string> SearchInitiated;

        public void Show()
        {
            // Reset the text box
            SearchBox.Text = "";

            // Show the search panel
            Visibility = Visibility.Visible;

            // Try focusing on the text box
            SearchBox.Focus();
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Initiate a search
                TextBox box = (TextBox)sender;

                if (SearchInitiated != null)
                {
                    SearchInitiated(this, box.Text);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
