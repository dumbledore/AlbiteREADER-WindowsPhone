using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public abstract class Entity
    {
        // Note
        // These are not mapped, so they *shan't* be used in LINQ queries
        public abstract int Id { get; protected set; }
        // Date Created
        // Data Updated
    }
}
