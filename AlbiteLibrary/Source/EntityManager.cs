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

        protected TEntity SetEntity(TEntity entity)
        {
            entity.Library = Library;
            return entity;
        }

        protected IList<TEntity> SetEntites(TEntity[] entities)
        {
            foreach (TEntity entity in entities)
            {
                SetEntity(entity);
            }

            return Array.AsReadOnly(entities);
        }

        public abstract void Remove(TEntity entity);
        public abstract TEntity this[int id] { get; }
        public abstract IList<TEntity> GetAll();
    }
}
