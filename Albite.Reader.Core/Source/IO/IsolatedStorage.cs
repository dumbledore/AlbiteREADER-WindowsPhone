﻿using System.IO;
using System.IO.IsolatedStorage;

namespace Albite.Reader.Core.IO
{
    public class IsolatedStorage : Storage
    {
        private readonly IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public IsolatedStorage(string filename) : base(normalize(filename)) { }

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

        public void CreatePathForFile()
        {
            createPathForFile(FileName);
        }

        private void createPathForFile(string fileName)
        {
            if (isf.FileExists(fileName))
            {
                // File already there
                return;
            }

            string strBaseDir = string.Empty;
            string[] dirNames = fileName.Split(separators);

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
                CreatePathForFile();
            }
            else
            {
                // Open mode is for reading only, so the file must exist
                if (!isf.FileExists(FileName))
                {
                    throw new FileNotFoundException(FileName);
                }
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

        protected override void MoveImplementation(string newFileName)
        {
            if (isf.DirectoryExists(FileName))
            {
                createPathForFile(newFileName);
                isf.MoveDirectory(FileName, newFileName);
            }
            else if (isf.FileExists(FileName))
            {
                createPathForFile(newFileName);
                isf.MoveFile(FileName, newFileName);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public override void Dispose()
        {
            isf.Dispose();
        }
    }
}
