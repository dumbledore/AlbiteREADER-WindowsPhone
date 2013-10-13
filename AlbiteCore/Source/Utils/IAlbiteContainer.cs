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
using System.IO;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public interface IAlbiteContainer : IDisposable
    {
        /// <summary>
        /// Gets the stream for the given entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        Stream Stream(string entityName);
    }
}
