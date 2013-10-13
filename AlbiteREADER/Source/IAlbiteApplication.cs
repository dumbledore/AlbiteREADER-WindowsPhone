using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SvetlinAnkov.Albite.READER
{
    public interface IAlbiteApplication : IDisposable
    {
        AlbiteContext CurrentContext { get; }

        void DisposeContext();
    }
}
