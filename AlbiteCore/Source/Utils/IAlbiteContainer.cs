using System;
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
