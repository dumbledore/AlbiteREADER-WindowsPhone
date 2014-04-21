using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Albite.Reader.Storage
{
    public abstract class StorageService
    {
        /// <summary>
        /// Service name, e.g. SD, OneDrive, etc.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Service id (should be unique)
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Service icon
        /// </summary>
        public virtual ImageSource Icon { get { return null; } }

        /// <summary>
        /// True if the user ought to log in before
        /// they can use the service
        /// </summary>
        public virtual bool LoginRequired { get { return false; } }

        /// <summary>
        /// Logs the user in
        /// </summary>
        public virtual Task LogIn()
        {
            throw new InvalidOperationException(NoLoginExceptionMessage);
        }

        /// <summary>
        /// Logs the user out
        /// </summary>
        public virtual void LogOut()
        {
            throw new InvalidOperationException(NoLoginExceptionMessage);
        }

        /// <summary>
        /// True if the user is currently logged in
        /// Throws InvalidOperationException if LoginRequired is false
        /// </summary>
        public virtual bool LoggedIn
        {
            get { throw new InvalidOperationException(NoLoginExceptionMessage); }
        }

        /// <summary>
        /// Retrieves the folder contents for a particular path
        /// </summary>
        /// <param name="path">The folder item. If null, it looks in the root folder.</param>
        /// <returns></returns>
        public abstract Task<ICollection<StorageItem>> GetFolderContentsAsync(StorageItem folder, CancellationToken ct);

        public Task<ICollection<StorageItem>> GetFolderContentsAsync(StorageItem folder)
        {
            return GetFolderContentsAsync(folder, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the contents of a file
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns></returns>
        public abstract Task<Stream> GetFileContentsAsync(
            StorageItem file, CancellationToken ct, IProgress<double> progress);

        /// <summary>
        /// Returns true if a file should be listed
        /// </summary>
        /// <param name="file">file name</param>
        /// <returns></returns>
        public delegate bool IsFileAccepted(string file);
        public IsFileAccepted IsFileAcceptedDelegate { get; set; }

        /// <summary>
        /// File icon
        /// </summary>
        public delegate ImageSource GetFileIcon();
        public GetFileIcon GetFileIconDelegate { get; set; }

        private static readonly string NoLoginExceptionMessage
            = "This service does not support authentication";
    }
}
