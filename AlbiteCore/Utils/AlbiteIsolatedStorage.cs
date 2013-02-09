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
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;
using System.Text;

namespace SvetlinAnkov.AlbiteREADER.Utils
{
    public class AlbiteIsolatedStorage : AlbiteStorage
    {
        public static readonly string Delimiter = "/";
        protected static readonly char[] DelimitarChars = Delimiter.ToCharArray();

        private readonly IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public AlbiteIsolatedStorage(string filename) : base(filename) { }

        private void createPathForFile()
        {
            if (isf.FileExists(FileName))
            {
                // File already there
                return;
            }

            int index = FileName.LastIndexOf(Delimiter);
            if (index < 0)
            {
                // No directories in the path
                return;
            }

            string path = FileName.Substring(0, index);
            if (isf.DirectoryExists(path))
            {
                // Directory already there
                return;
            }

            string strBaseDir = string.Empty;
            char[] delimiter = DelimitarChars;
            string[] dirNames = path.Split(delimiter);

            for (int i = 0; i < dirNames.Length; i++)
            {
                strBaseDir = System.IO.Path.Combine(strBaseDir, dirNames[i]);
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

        public override void Dispose()
        {
            isf.Dispose();
        }
    }
}
