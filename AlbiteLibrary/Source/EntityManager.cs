using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library
{
    public abstract class EntityManager<TEntity>
        where TEntity : Entity
    {
        protected readonly Library Library;

        public EntityManager(Library library)
        {
            Library = library;
        }

        protected sealed TEntity PrepareEntity(TEntity entity)
        {
            entity.Library = Library;
            return entity;
        }

        protected sealed IList<TEntity> PrepareEntities(TEntity[] entities)
        {
            foreach (TEntity entity in entities)
            {
                PrepareEntity(entity);
            }

            return Array.AsReadOnly(entities);
        }

        public abstract void Remove(TEntity entity);
        public abstract TEntity this[int id] { get; }
        public abstract IList<TEntity> GetAll();
    }
}
