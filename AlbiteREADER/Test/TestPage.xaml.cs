using Microsoft.Phone.Controls;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void AutomatedTestsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new Tests().Test();
        }
    }
}