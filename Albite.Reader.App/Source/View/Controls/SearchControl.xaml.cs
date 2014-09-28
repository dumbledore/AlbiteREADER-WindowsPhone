using System;
using System.Windows;
using System.Windows.Controls;
using Windows.Foundation;
using System.Windows.Input;
using Windows.Phone.Speech.Recognition;
using Albite.Reader.Core.Collections;

namespace Albite.Reader.App.View.Controls
{
    public partial class SearchControl : UserControl, IDisposable
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        public event TypedEventHandler<SearchControl, string> SearchInitiated;

        private readonly CachedRecognizer cachedRecognizer = new CachedRecognizer();

        public string Text
        {
            get { return SearchBox.Text; }
            set { SearchBox.Text = value; }
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Get the textbox
                TextBox box = (TextBox)sender;

                // Initiate a search
                initiateSearch(box.Text);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            SpeechRecognizerUI recognizer = cachedRecognizer.Value;
            SpeechRecognitionUIResult result = await recognizer.RecognizeWithUIAsync();

            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                string searchText = result.RecognitionResult.Text;

                // Remove the annoying period
                if (searchText.EndsWith(".") && searchText.Length > 1)
                {
                    searchText = searchText.Substring(0, searchText.Length - 1);
                }

                // Update text box
                SearchBox.Text = searchText;

                // Initiate search
                initiateSearch(searchText);
            }
            else
            {
                // Failed or cancel.
                // In either case return focus to the textbox
                SearchBox.Focus();
            }
        }

        private void initiateSearch(string searchString)
        {
            // Cache the min chars
            int min = MinChars;

            if (min > 0 && searchString.Length < min)
            {
                // Inform the user
                MessageBox.Show(
                    string.Format(
                    "You need to enter at least {0} characters",
                    min));

                // Back to text
                SearchBox.Focus();
            }
            else
            {

                if (SearchInitiated != null)
                {
                    SearchInitiated(this, searchString);
                }
            }
        }

        private class CachedRecognizer : DisposableCachedObject<SpeechRecognizerUI>
        {
            protected override SpeechRecognizerUI GenerateValue()
            {
                SpeechRecognizerUI recognizer = new SpeechRecognizerUI();
                recognizer.Settings.ExampleText = "Adventures of Sherlock Holmes";
                recognizer.Settings.ReadoutEnabled = true;
                recognizer.Settings.ShowConfirmation = true;
                return recognizer;
            }
        }

        public void Dispose()
        {
            cachedRecognizer.Dispose();
        }

        public static readonly DependencyProperty MinCharsProperty =
            DependencyProperty.Register("MinChars", typeof(int), typeof(SearchControl), null);

        public int MinChars
        {
            get { return (int)GetValue(MinCharsProperty); }
            set { SetValue(MinCharsProperty, value); }
        }
    }
}
