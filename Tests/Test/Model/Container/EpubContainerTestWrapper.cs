using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.Tests.Test.Model.Container
{
    public class EpubContainerTestWrapper : TestPrepareWrapper
    {
        private string epubPath;

        public EpubContainerTestWrapper(string epubPath)
            : base(new EpubContainerTest(epubPath))
        {
            this.epubPath = epubPath;
        }

        protected override void onTearUp()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(epubPath))
            {
                // Copy from res to iso
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(epubPath))
                {
                    res.CopyTo(iso);
                }
            }
        }

        protected override void onTearDown()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(epubPath))
            {
                // Delete iso
                iso.Delete();
            }
        }
    }
}
