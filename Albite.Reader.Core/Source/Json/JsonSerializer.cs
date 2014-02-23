using Albite.Reader.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Albite.Reader.Core.Json
{
    public class JsonSerializer<TEntity> : Serializer<TEntity>
    {
        private readonly DataContractJsonSerializer serializer;

        public JsonSerializer(IEnumerable<Type> entityTypes)
        {
            serializer = new DataContractJsonSerializer(
                typeof(TEntity), entityTypes);
        }

        protected override void WriteObject(Stream stream, TEntity entity)
        {
            serializer.WriteObject(stream, entity);
        }

        protected override TEntity ReadObject(Stream stream)
        {
            return (TEntity) serializer.ReadObject(stream);
        }
    }
}
