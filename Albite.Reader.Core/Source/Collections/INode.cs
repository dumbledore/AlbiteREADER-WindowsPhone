namespace Albite.Reader.Core.Collections
{
    public interface INode<TValue>
    {
        INode<TValue> FirstChild { get; }

        INode<TValue> NextSibling { get; }

        int Depth { get; }

        TValue Value { get; }
    }
}
