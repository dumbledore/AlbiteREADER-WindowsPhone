using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library
{
    public abstract class LibraryEntity : Entity
    {
        private Library library;
        public Library Library
        {
            get
            {
                if (library == null)
                {
                    throw new EntityInvalidException("Not attached to a library");
                }

                return library;
            }

            set { library = value; }
        }

        protected LibraryEntity(Library library, Entity dataEntity)
        {
            Library = library;
            Id = dataEntity.Id;
            //TODO other DataEntity fields
        }

        public abstract void Remove();
    }
}
