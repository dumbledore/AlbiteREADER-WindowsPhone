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
using SvetlinAnkov.AlbiteREADER.Utils;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using SvetlinAnkov.AlbiteREADER.Utils.Xml;

namespace SvetlinAnkov.AlbiteREADER.Model.Container.Epub
{
    public class EpubContainer : BookContainer
    {
        public OpenContainerFile Ocf { get; private set; }
        public OpenPackageFile Opf { get; private set; }
        public NavigationControlFile Ncx { get; private set; }

        public EpubContainer(IAlbiteContainer archive) : base(archive)
        {
            try
            {
                // Read the container and extract the location to the OPF
                Ocf = new OpenContainerFile(container);

                // Read the metadata, manifest & spine.
                // Note that OpfPath is relative to the root, not to META-INF.
                Opf = new OpenPackageFile(container, Ocf.OpfPath);

                // Read the table of contents.
                Ncx = new NavigationControlFile(container, Opf.NcxPath);
            }
            catch (Exception e)
            {
                throw new BookContainerException("Processing the ePub container failed", e);
            }
        }

        public override void Install(AlbiteStorage outputStorage)
        {
            // Simply copy the entities to the storage
            //IList<string> names = items;
            //foreach (string name in entityNames)
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
    }
}
