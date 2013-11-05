using System.IO;
using System.Text;

namespace SvetlinAnkov.Albite.Core.Serialization
{
    public abstract class AlbiteSerializer<TEntity>
    {
        public string Encode(TEntity entity)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteObject(stream, entity);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public TEntity Decode(string data)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return ReadObject(stream);
            }
        }

        protected abstract void WriteObject(Stream stream, TEntity entity);
        protected abstract TEntity ReadObject(Stream stream);
    }
}
