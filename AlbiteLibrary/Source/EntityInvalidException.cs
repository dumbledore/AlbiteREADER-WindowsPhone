using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class EntityInvalidException : Exception
    {
        public EntityInvalidException() : base() { }
        public EntityInvalidException(string message) : base(message) { }
        public EntityInvalidException(string message, Exception innerException) : base(message, innerException) { }
    }
}
