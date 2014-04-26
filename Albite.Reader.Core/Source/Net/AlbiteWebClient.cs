using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.Core.Net
{
    public class AlbiteWebClient
    {
        TaskCompletionSource<Stream> currentTaskSource;
        IProgress<double> currentProgress;

        private WebClient client = new WebClient();
        private object myLock = new Object();

        public AlbiteWebClient()
        {
            client.DownloadProgressChanged += downloadProgressChanged;
            client.OpenReadCompleted += readCompleted;
        }

        private void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (myLock)
            {
                if (currentProgress != null)
                {
                    currentProgress.Report(e.ProgressPercentage);
                }
            }
        }

        private void readCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            lock (myLock)
            {
                if (currentTaskSource != null)
                {
                    if (e.Cancelled)
                    {
                        currentTaskSource.SetCanceled();
                    }
                    else if (e.Error != null)
                    {
                        currentTaskSource.SetException(e.Error);
                    }
                    else
                    {
                        currentTaskSource.SetResult(e.Result);
                    }

                    currentProgress = null;
                    currentTaskSource = null;
                }
            }
        }

        public Task<Stream> DownloadAsync(Uri uri, CancellationToken ct)
        {
            lock (myLock)
            {
                if (currentTaskSource != null)
                {
                    throw new InvalidOperationException("Download is still running");
                }

                currentTaskSource = new TaskCompletionSource<Stream>();

                // Call CancelAsync on cancellation
                if (ct != CancellationToken.None)
                {
                    ct.Register(() =>
                    {
                        client.CancelAsync();
                    });
                }

                // Start the download
                client.OpenReadAsync(uri);

                return currentTaskSource.Task;
            }
        }
    }
}
