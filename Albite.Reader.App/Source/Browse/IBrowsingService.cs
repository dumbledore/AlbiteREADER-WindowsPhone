using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace Albite.Reader.App.Browse
{
    public interface IBrowsingService
    {
        /// <summary>
        /// Service name, e.g. SD, OneDrive, etc.
        /// </summary>
        string Name { get; }

        string Id { get; }

        ImageSource Icon { get; }

        /// <summary>
        /// True if the user ought to log in before
        /// they can use the service
        /// </summary>
        bool LoginRequired { get; }

        /// <summary>
        /// Logs the user in
        /// </summary>
        void LogIn();

        /// <summary>
        /// Logs the user out
        /// </summary>
        void LogOut();

        /// <summary>
        /// True if the user is currently logged in
        /// Throws InvalidOperationException if LoginRequired is false
        /// </summary>
        bool LoggedIn { get; }

        /// <summary>
        /// Retrieves the folder contents for a particular path
        /// </summary>
        /// <param name="path">The folder path</param>
        /// <returns></returns>
        ICollection<IFolderItem> GetFolderContentsAsync(string path);

        /// <summary>
        /// Retrieves the contents of a file
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns></returns>
        Stream GetFileContentsAsync(string path);
    }
}
