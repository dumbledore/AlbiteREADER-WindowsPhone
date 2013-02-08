using Microsoft.Phone.Controls;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
            new Tests().Test();
        }
    }
}