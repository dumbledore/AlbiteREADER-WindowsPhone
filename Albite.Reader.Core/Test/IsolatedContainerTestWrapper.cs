using Albite.Reader.Core.IO;
using System.IO;

namespace Albite.Reader.Core.Test
{
    public class IsolatedContainerTestWrapper : TestPrepareWrapper
    {
        private string zipPath;
        private string[] zipEntityNames;

        public IsolatedContainerTestWrapper(
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

        private static IsolatedContainerTest createTest(string zipPath, string[] entityNames)
        {
            return new IsolatedContainerTest(getInstallPath(zipPath), entityNames);
        }

        protected override void onTearUp()
        {
            using (IsolatedStorage iso = new IsolatedStorage(zipPath))
            {
                // Copy from res to iso
                using (ResourceStorage res = new ResourceStorage(zipPath))
                {
                    res.CopyTo(iso);
                }

                using (Stream inputStream = iso.GetStream(FileAccess.Read))
                {
                    using (ZipContainer zip = new ZipContainer(inputStream))
                    {
                        // Unpack it
                        string installPath = getInstallPath(zipPath);

                        foreach (string zipEntity in zipEntityNames)
                        {
                            using (IsolatedStorage output = new IsolatedStorage(System.IO.Path.Combine(installPath, zipEntity)))
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
            using (IsolatedStorage dir = new IsolatedStorage(getInstallPath(zipPath)))
            {
                dir.Delete();
            }

            // Delete iso file
            using (IsolatedStorage iso = new IsolatedStorage(zipPath))
            {
                iso.Delete();
            }
        }
    }
}
