using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.Container.Epub
{
    public class EpubContainer : BookContainer
    {
        private static readonly string tag = "EpubContainer";

        private IHashableContainer container;

        internal OpenContainerFile Ocf { get; private set; }
        internal OpenPackageFile Opf { get; private set; }
        internal NavigationControlFile Ncx { get; private set; }

        /// <summary>
        /// Creates a instance of EpubContainer
        /// </summary>
        /// <param name="container">The source IContainer</param>
        /// <param name="fallback">
        ///     If true, non-fatal errors when using this container won't cause
        ///     an exception. Use HadErrors to check if there were any.
        /// </param>
        public EpubContainer(IHashableContainer container, bool fallback = true) : base(fallback)
        {
            this.container = container;
            processDocuments();
        }

        private string[] items_;

        private string[] getItems()
        {
            if (items_ == null)
            {
                // Copy the items from the manifest
                List<string> itemsRes = new List<string>(Opf.Items);

                // Don't forget to add the OCF, OPF and NCX
                itemsRes.Add(OpenContainerFile.Path);
                itemsRes.Add(Ocf.OpfPath);
                if (Opf.NcxPath != null)
                {
                    itemsRes.Add(Opf.NcxPath);
                }

                items_ = itemsRes.ToArray();
            }

            return items_;
        }

        public override IEnumerable<string> Items
        {
            get { return getItems(); }
        }

        public override IEnumerable<String> Spine
        {
            get { return Opf.Spine; }
        }

        public override string Title
        {
            get { return Opf.Title; }
        }

        public override string Author
        {
            get { return Opf.Author; }
        }

        public override string Cover
        {
            get { return Opf.Cover; }
        }

        public override Core.Collections.ITree<IContentItem> Contents
        {
            get { return Ncx.NavigationMap; }
        }

        public override Task<bool> InstallAsync(
            string path, CancellationToken cancelToken, IProgress<double> progress)
        {
            return Task<bool>.Run(() =>
                {
                    return Install(path, cancelToken, progress);
                }, cancelToken);
        }

        public override bool Install(string path)
        {
            return Install(path, CancellationToken.None, null);
        }

        private bool Install(
            string path, CancellationToken cancelToken, IProgress<double> progress)
        {
            bool hadErrors = false;
            string[] items = getItems();

            int i = 0;
            double itemCount = items.Length;

            foreach (string item in items)
            {
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    using (IsolatedStorage output = new IsolatedStorage(System.IO.Path.Combine(path, item)))
                    {
                        using (Stream input = Stream(item))
                        {
                            output.Write(input);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.E(tag, "Failed unpacking " + item, e);
                    hadErrors = true;

                    if (!Fallback)
                    {
                        throw e;
                    }
                }

                if (progress != null)
                {
                    // if we are in a task, report the
                    // current normalized progress
                    progress.Report(++i / itemCount);
                }
            }

            return hadErrors;
        }

        public override Stream Stream(string entityName)
        {
            // First check that this stream is there and/or is allowed to
            // be used at all.
            if (Opf.ContainsItem(entityName)
                || entityName == OpenContainerFile.Path
                || entityName == Ocf.OpfPath
                || entityName == Opf.NcxPath)
            {
                return container.Stream(entityName);
            }

            throw new BookContainerException("Entity not found in book");
        }

        public override byte[] ComputeHash(HashAlgorithm hashAlgorithm)
        {
            return container.ComputeHash(hashAlgorithm);
        }

        public override void Dispose()
        {
            container.Dispose();
        }

        private void processDocuments()
        {
            try
            {
                // Read the container and extract the location to the OPF
                Ocf = new OpenContainerFile(container);

                // Read the manifest, spine & (optionally) metadata, .
                Opf = new OpenPackageFile(container, Ocf.OpfPath);

                // Read the table of contents. Don't crash on error.
                try
                {
                    if (Opf.NcxPath != null)
                    {
                        Ncx = new NavigationControlFile(container, Opf.NcxPath);
                    }
                    else
                    {
                        Log.W(tag, "NCX not available");
                    }
                }
                catch (Exception e)
                {
                    Log.W(tag, "Couldn't parse the ncx", e);
                }
            }
            catch (Exception e)
            {
                Log.W(tag, "Couldn't create the ePub container", e);
                throw new BookContainerException("Processing the ePub container failed", e);
            }

            // Summarize the problems
            HadErrors |= Opf.HadErrors || Ncx == null || Ncx.HadErrors;
        }
    }
}
