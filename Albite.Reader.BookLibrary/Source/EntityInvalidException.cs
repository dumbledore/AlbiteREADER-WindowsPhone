using System;

namespace Albite.Reader.BookLibrary
{
    public class EntityInvalidException : Exception
    {
        public EntityInvalidException() : base() { }
        public EntityInvalidException(string message) : base(message) { }
        public EntityInvalidException(string message, Exception innerException) : base(message, innerException) { }
    }
}
