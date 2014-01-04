namespace SvetlinAnkov.Albite.Core.Collections
{
    public interface INode<TValue>
    {
        INode<TValue> FirstChild { get; }

        INode<TValue> NextSibling { get; }

        TValue Value { get; }
    }
}
