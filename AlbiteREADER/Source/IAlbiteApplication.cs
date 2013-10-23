using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SvetlinAnkov.Albite.READER
{
    public interface IAlbiteApplication
    {
        AlbiteContext CurrentContext { get; }
    }
}
