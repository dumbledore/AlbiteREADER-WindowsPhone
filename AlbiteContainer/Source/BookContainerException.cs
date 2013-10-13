using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Container
{
    public class BookContainerException : Exception
    {
        public BookContainerException(string message) : base(message) { }
        public BookContainerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
