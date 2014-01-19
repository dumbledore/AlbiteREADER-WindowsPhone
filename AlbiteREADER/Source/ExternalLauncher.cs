using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System;

namespace SvetlinAnkov.Albite.READER
{
    public static class ExternalLauncher
    {
        public static async Task LaunchAppDetailsPage()
        {
            string uri = String.Format("zune:navigate?appid={0}", CurrentApp.AppId);
            await Launcher.LaunchUriAsync(new Uri(uri));
        }

        public static async Task LaunchAppRatePage()
        {
            await Launcher.LaunchUriAsync(new Uri("zune:reviewapp"));
        }
    }
}
