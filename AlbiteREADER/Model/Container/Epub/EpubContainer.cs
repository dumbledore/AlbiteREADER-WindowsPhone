using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.Core.Utils.Xml;

namespace SvetlinAnkov.Albite.READER.Model.Container.Epub
{
    public class EpubContainer : BookContainer
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
            get { return Opf.Items; }
        }

        public override string Title
        {
            get { return Opf.Title; }
        }

        public override Stream Stream(string entityName)
        {
            // First check that this stream is there and/or is allowed to
            // be used at all.
            if (!Opf.ContainsItem(entityName))
            {
                throw new BookContainerException("Entity not found in book");
            }

            return base.Stream(entityName);
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
