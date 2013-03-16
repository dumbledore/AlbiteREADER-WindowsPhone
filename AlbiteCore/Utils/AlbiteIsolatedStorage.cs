using System;
using System.Net;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;
using System.Text;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public class AlbiteIsolatedStorage : AlbiteStorage
    {
        private readonly IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public AlbiteIsolatedStorage(string filename) : base(normalize(filename)) { }

        private static char[] separators
            = { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar };

        // Calling IsolatedStorageFile.Delete() for paths
        // that end with a separator throws an exception.
        // This doesn't look like correct behaviour, but
        // has to be circumvented anyway.
        private static string normalize(string filename)
        {
            return filename.TrimEnd(separators);
        }

        private void createPathForFile()
        {
            if (isf.FileExists(FileName))
            {
                // File already there
                return;
            }

            string strBaseDir = string.Empty;
            string[] dirNames = FileName.Split(separators);

            for (int i = 0; i < dirNames.Length - 1; i++)
            {
                strBaseDir = Path.Combine(strBaseDir, dirNames[i]);
                isf.CreateDirectory(strBaseDir);
            }
        }

        public override Stream GetStream(FileAccess access, FileMode mode, FileShare share)
        {
            if (mode != FileMode.Open)
            {
                createPathForFile();
            }

            return isf.OpenFile(FileName, mode, access, share);
        }

        public override void Delete()
        {
            if (isf.FileExists(FileName))
            {
                // there's a file, delete it.
                isf.DeleteFile(FileName);
                return;
            }

            if (!isf.DirectoryExists(FileName))
            {
                // there isn't such a dir, nothing to do
                return;
            }

            delete(FileName);
        }

        private void delete(string dir)
        {
            string pattern = Path.Combine(dir, "*");

            // First delete all files
            {
                string[] files = isf.GetFileNames(pattern);
                foreach (string f in files)
                {
                    isf.DeleteFile(Path.Combine(dir, f));
                }
            }

            // Now start with the subdirs
            {
                string[] dirs = isf.GetDirectoryNames(pattern);
                foreach (string d in dirs)
                {
                    delete(Path.Combine(dir, d));
                }
            }

            // Ready to delete this dir
            isf.DeleteDirectory(dir);
        }

        public override void Dispose()
        {
            isf.Dispose();
        }
    }
}
