using Albite.Reader.Core.Diagnostics;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.Core.Net
{
    public class AlbiteWebClient
    {
        private static readonly string Tag = "AlbiteWebClient";

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

                Log.D(Tag, "Downloaded " + e.ProgressPercentage + "%");
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
                        Log.D(Tag, "Cancelled");
                        currentTaskSource.SetCanceled();
                    }
                    else if (e.Error != null)
                    {
                        Log.D(Tag, "Error occured", e.Error);
                        currentTaskSource.SetException(e.Error);
                    }
                    else
                    {
                        Log.D(Tag, "Completed");
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
