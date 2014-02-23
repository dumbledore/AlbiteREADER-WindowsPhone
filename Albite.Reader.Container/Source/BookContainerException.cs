using System;

namespace Albite.Reader.Container
{
    public class BookContainerException : Exception
    {
        public BookContainerException(string message) : base(message) { }
        public BookContainerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
