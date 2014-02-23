namespace Albite.Reader.Core.Serialization
{
    public interface ISerializer<TEntity>
    {
        string Encode(TEntity entity);
        TEntity Decode(string data);
    }
}
