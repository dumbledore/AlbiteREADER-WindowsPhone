using Microsoft.Phone.Controls;
using Albite.Reader.App.Browse;
using System.Collections.ObjectModel;
using Albite.Reader.App.View.Controls;
using System;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class BrowsingServicesPage : PhoneApplicationPage
    {
        public BrowsingServicesPage()
        {
            InitializeComponent();

            // Add services
            ObservableCollection<IBrowsingService> services =
                new ObservableCollection<IBrowsingService>(
                    BrowsingServices.Services);

            SourcesList.ItemsSource = services;
        }

        private void Control_Tap(object sender, GEArgs e)
        {
            BrowsingServiceControl control = (BrowsingServiceControl)sender;
            IBrowsingService service = control.BrowsingService;
            NavigationService.Navigate(new Uri(
                "/Albite.Reader.App;component/Source/View/Pages/BrowsePage.xaml?service="
                + service.Id, UriKind.Relative));
        }
    }
}