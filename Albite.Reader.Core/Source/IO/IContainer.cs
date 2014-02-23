using System;
using System.IO;

namespace Albite.Reader.Core.IO
{
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// Gets the stream for the given entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        Stream Stream(string entityName);
    }
}
