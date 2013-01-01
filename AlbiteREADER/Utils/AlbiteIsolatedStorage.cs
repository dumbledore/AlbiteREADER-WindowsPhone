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
        private IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public AlbiteIsolatedStorage(string filename) : base(filename) { }

        public override AlbiteStorage OpenRelative(string filename)
        {
            return new AlbiteIsolatedStorage(this.filename + filename);
        }

        protected override void CreatePathForFile()
        {
            if (isf.FileExists(filename))
            {
                // File already there
                return;
            }

            int index = filename.LastIndexOf(Delimiter);
            if (index < 0)
            {
                // No directories in the path
                return;
            }

            string path = filename.Substring(0, index);
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

        protected override Stream getStream(FileMode fileMode)
        {
            return isf.OpenFile(filename, fileMode);
        }

        public override void Dispose()
        {
            isf.Dispose();
        }
    }
}
