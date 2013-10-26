using SvetlinAnkov.Albite.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace SvetlinAnkov.Albite.Container.Epub
{
    internal class EpubContainer : BookContainer
    {
        private static readonly string tag = "EpubContainer";

        public OpenContainerFile Ocf { get; private set; }
        public OpenPackageFile Opf { get; private set; }
        public NavigationControlFile Ncx { get; private set; }

        public EpubContainer(IAlbiteContainer container, bool fallback = true) : base(container, fallback)
        {
            processDocuments();
        }

        public override IEnumerable<string> Items
        {
            get
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

                return itemsRes;
            }
        }

        public override IEnumerable<String> Spine
        {
            get { return Opf.Spine; }
        }

        public override string Title
        {
            get { return Opf.Title; }
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
                return base.Stream(entityName);
            }

            throw new BookContainerException("Entity not found in book");
        }

        private void processDocuments()
        {
            try
            {
                // Read the container and extract the location to the OPF
                Ocf = new OpenContainerFile(Container);

                // Read the manifest, spine & (optionally) metadata, .
                Opf = new OpenPackageFile(Container, Ocf.OpfPath);

                // Read the table of contents. Don't crash on error.
                try
                {
                    if (Opf.NcxPath != null)
                    {
                        Ncx = new NavigationControlFile(Container, Opf.NcxPath);
                    }
                    else
                    {
                        Log.E(tag, "NCX not available");
                    }
                }
                catch (Exception e)
                {
                    Log.E(tag, "Couldn't parse the ncx", e);
                }
            }
            catch (Exception e)
            {
                Log.E(tag, "Couldn't create the ePub container", e);
                throw new BookContainerException("Processing the ePub container failed", e);
            }

            // Summarize the problems
            HadErrors |= Opf.HadErrors || Ncx == null || Ncx.HadErrors;
        }
    }
}
