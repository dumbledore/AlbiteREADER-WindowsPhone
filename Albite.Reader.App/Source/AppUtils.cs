using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;

namespace Albite.Reader.App
{
    public class AppUtils
    {
        public static PhoneApplicationPage CurrentPage
        {
            get
            {
                Application current = Application.Current;
                if (current == null)
                {
                    return null;
                }

                PhoneApplicationFrame frame = current.RootVisual as PhoneApplicationFrame;
                if (frame == null)
                {
                    return null;
                }

                return frame.Content as PhoneApplicationPage;
            }
        }

        public static IApplicationBar ApplicationBar
        {
            get
            {
                PhoneApplicationPage currentPage = CurrentPage;
                if (currentPage == null)
                {
                    return null;
                }

                return currentPage.ApplicationBar;
            }
        }
    }
}
