namespace SvetlinAnkov.Albite.Core.Collections
{
    public interface ITree<TValue>
    {
        INode<TValue> Root { get; }
    }
}
