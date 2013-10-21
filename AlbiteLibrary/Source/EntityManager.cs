using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library
{
    public abstract class EntityManager<TEntity>
        where TEntity : LibraryEntity
    {
        protected readonly Library Library;

        public EntityManager(Library library)
        {
            Library = library;
        }

        public abstract void Remove(TEntity entity);
        public abstract TEntity this[int id] { get; }
        public abstract IList<TEntity> GetAll();
    }
}
