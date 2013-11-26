namespace SvetlinAnkov.Albite.Core.Serialization
{
    public interface IAlbiteSerializer<TEntity>
    {
        string Encode(TEntity entity);
        TEntity Decode(string data);
    }
}
