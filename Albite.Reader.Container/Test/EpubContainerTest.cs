﻿using Albite.Reader.Container.Epub;
using Albite.Reader.Core.Collections;
using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;
using System.IO;

namespace Albite.Reader.Container.Test
{
    public class EpubContainerTest : TestCase
    {
        private string epubPath;

        public EpubContainerTest(string epubPath)
        {
            this.epubPath = epubPath;
        }

        protected override void  TestImplementation()
        {
            Log("Opening ePub {0}", epubPath);

            using (IsolatedStorage iso = new IsolatedStorage(epubPath))
            {
                using (Stream inputStream = iso.GetStream(FileAccess.Read))
                {
                    using (ZipContainer zip = new ZipContainer(inputStream))
                    {
                        // The default path for installation
                        string path = epubPath + "_install/";

                        // Create the ePub
                        EpubContainer epub = new EpubContainer(zip);

                        // Dump it
                        dumpEpub(epub);

                        // Unpack it
                        epub.Install(path);

                        // Remove it
                        using (IsolatedStorage dir = new IsolatedStorage(path))
                        {
                            dir.Delete();
                        }
                    }
                }
            }
        }

        private void dumpEpub(EpubContainer epub)
        {
            dumpOcf(epub.Ocf);
            dumpOpf(epub.Opf);
            dumpNcx(epub.Ncx);

            Log("Errors opening {0}: {1}", epubPath,
                epub.HadErrors ? "y" : "n");
        }

        private void dumpOcf(OpenContainerFile ocf)
        {
            Log("Opf Path: {0}", ocf.OpfPath);
        }

        private void dumpOpf(OpenPackageFile opf)
        {
            // Metadata
            Log("Author: {0}", opf.Author);
            Log("Title: {0}", opf.Title);
            Log("Language: {0}", opf.Language);
            Log("Publisher: {0}", opf.Publisher);
            Log("Publication Date: {0}", opf.PublicationDate);
            Log("Rights: {0}", opf.Rights);

            // Manifest
            foreach (string id in opf.ItemIds)
            {
                Log("Item {0} => {1}", id, opf.Item(id));
            }

            // Spine
            foreach (string id in opf.Spine)
            {
                Log("Spine {0} => {1}", id, opf.Item(id));
            }

            // Ncx location
            Log("Ncx Path: {0}", opf.NcxPath);
        }

        private void dumpNcx(NavigationControlFile ncx)
        {
            if (ncx == null)
            {
                Log("No NCX available");
                return;
            }

            // Dump the navigation map
            if (ncx.NavigationMap != null)
            {
                foreach (INode<IContentItem> node in ncx.NavigationMap)
                {
                    Log("NavPoint. Label: {0}, Content: {1}",
                        node.Value.Title, node.Value.Location);
                }
            }

            // Dump the navigation lists
            if (ncx.NavigationLists != null)
            {
                foreach (NavigationControlFile.NavList navList in ncx.NavigationLists)
                {
                    dumpNavList(navList);
                }
            }

            // Dump the guide refs
            if (ncx.GuideReferences != null)
            {
                foreach (NavigationControlFile.GuideReference guideRef in ncx.GuideReferences)
                {
                    dumpGuideReference(guideRef);
                }
            }
        }

        private void dumpNavList(NavigationControlFile.NavList navList)
        {
            Log("NavList {0}", navList.Label);

            NavigationControlFile.NavTarget navTarget = navList.FirstTarget;

            while (navTarget != null)
            {
                Log("NavTarget. Label: {0}, Content: {1}",
                    navTarget.Label, navTarget.Src);

                navTarget = navTarget.NextSibling;
            }
        }

        private void dumpGuideReference(NavigationControlFile.GuideReference guideReference)
        {
            Log("Guide Reference. Type: {0} Title: {1} Href: {2}",
                guideReference.GuideType, guideReference.Title, guideReference.Href);
        }
    }
}
