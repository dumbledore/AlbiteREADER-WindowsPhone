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

namespace SvetlinAnkov.AlbiteREADER.Model.Containers.Epub
{
    public class EpubContainer : BookContainer
    {
        public OpenContainerFile Ocf { get; private set; }
        public OpenPackageFile Opf { get; private set; }
        public NavigationControlFile Ncx { get; private set; }

        public EpubContainer(IAlbiteContainer archive) : base(archive)
        {
            // Read the container and extract the location to the OPF
            Ocf = readContainerFile(OpenContainerFile.Path);

            // Read the metadata, manifest & spine
            Opf = readPackageFile(Ocf.OpfPath);

            // Read the table of contents
            //Ncx = readNavigationFile(Opf.NcxPath);
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

        private OpenContainerFile readContainerFile(string filename)
        {
            try
            {
                using (Stream stream = container.Stream(filename))
                {
                    return new OpenContainerFile(XDocument.Load(stream));
                }
            }
            catch (Exception e)
            {
                throw new BookContainerException("Processing the OCF failed", e);
            }
        }

        private OpenPackageFile readPackageFile(string filename)
        {
            try
            {
                using (Stream stream = container.Stream(filename))
                {
                    return new OpenPackageFile(XDocument.Load(stream));
                }
            }
            catch (Exception e)
            {
                throw new BookContainerException("Processing the OPF failed", e);
            }
        }

        private NavigationControlFile readNavigationFile(string filename)
        {
            try
            {
                using (Stream stream = container.Stream(filename))
                {
                    return new NavigationControlFile(XDocument.Load(stream));
                }
            }
            catch (Exception e)
            {
                throw new BookContainerException("Processing the NCX failed", e);
            }
        }
    }
}
