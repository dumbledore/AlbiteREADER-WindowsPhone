namespace Albite.Reader.Core.Xml.Atom
{
    internal class AtomEntity
    {
        public string Title { get; private set; }

        public AtomEntity(string title)
        {
            Title = title;
        }
    }
}
