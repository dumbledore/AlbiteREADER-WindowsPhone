using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System;

namespace Albite.Reader.App
{
    public static class ExternalLauncher
    {
        private static Uri getUri(string action)
        {
            return new Uri(
                    String.Format("zune:{0}?appid={1}", action, CurrentApp.AppId));
        }

        public static async Task LaunchAppDetailsPage()
        {
            await Launcher.LaunchUriAsync(getUri("navigate"));
        }

        public static async Task LaunchAppRatePage()
        {
            await Launcher.LaunchUriAsync(getUri("reviewapp"));
        }
    }
}
