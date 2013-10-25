using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public abstract class EntityManager<TEntity>
        where TEntity : LibraryEntity
    {
        public Library Library { get; private set; }

        public EntityManager(Library library)
        {
            Library = library;
        }

        public abstract void Remove(TEntity entity);
        public abstract TEntity this[int id] { get; }
        public abstract IList<TEntity> GetAll();
    }
}
