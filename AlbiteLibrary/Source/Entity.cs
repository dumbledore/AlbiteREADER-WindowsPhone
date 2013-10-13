using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library
{
    public abstract class Entity
    {
        public abstract int Id { get; protected set; }
        // Date Created
        // Data Updated
        // Name

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

        public abstract void Persist();
        public abstract void Remove();
    }
}
