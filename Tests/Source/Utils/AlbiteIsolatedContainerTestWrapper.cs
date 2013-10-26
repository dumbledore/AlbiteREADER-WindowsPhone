using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Core.Test;
using System.IO;

namespace SvetlinAnkov.Albite.Tests.Utils
{
    public class AlbiteIsolatedContainerTestWrapper : TestPrepareWrapper
    {
        private string zipPath;
        private string[] zipEntityNames;

        public AlbiteIsolatedContainerTestWrapper(
            string zipPath, string[] zipEntityNames, string[] containerEntityNames)

            : base(createTest(zipPath, containerEntityNames))
        {
            this.zipPath = zipPath;
            this.zipEntityNames = zipEntityNames;
        }

        private static string getInstallPath(string zipPath)
        {
            return zipPath + "_install/";
        }

        private static AlbiteIsolatedContainerTest createTest(string zipPath, string[] entityNames)
        {
            return new AlbiteIsolatedContainerTest(getInstallPath(zipPath), entityNames);
        }

        protected override void onTearUp()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(zipPath))
            {
                // Copy from res to iso
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(zipPath))
                {
                    res.CopyTo(iso);
                }

                using (Stream inputStream = iso.GetStream(FileAccess.Read))
                {
                    using (AlbiteZipContainer zip = new AlbiteZipContainer(inputStream))
                    {
                        // Unpack it
                        string installPath = getInstallPath(zipPath);

                        foreach (string zipEntity in zipEntityNames)
                        {
                            using (AlbiteIsolatedStorage output = new AlbiteIsolatedStorage(System.IO.Path.Combine(installPath, zipEntity)))
                            {
                                using (Stream input = zip.Stream(zipEntity))
                                {
                                    output.Write(input);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void onTearDown()
        {
            // Delete iso dir
            using (AlbiteIsolatedStorage dir = new AlbiteIsolatedStorage(getInstallPath(zipPath)))
            {
                dir.Delete();
            }

            // Delete iso file
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(zipPath))
            {
                iso.Delete();
            }
        }
    }
}
