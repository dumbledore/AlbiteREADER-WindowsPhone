using System;

namespace Albite.Reader.Core.Xml.Atom
{
    public class AtomException : Exception
    {
        public AtomException(string message) : base(message) { }
        public AtomException(string message, Exception innerException) : base(message, innerException) { }
    }
}