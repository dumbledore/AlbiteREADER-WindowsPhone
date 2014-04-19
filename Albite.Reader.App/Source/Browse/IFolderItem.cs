using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albite.Reader.App.Browse
{
    public interface IFolderItem
    {
        string Name { get; }
        bool IsFile { get; }
    }
}
