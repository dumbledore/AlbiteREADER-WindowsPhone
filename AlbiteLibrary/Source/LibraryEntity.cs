﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library
{
    public abstract class LibraryEntity : Entity
    {
        public Library Library { get; private set; }

        protected LibraryEntity(Library library, Entity dataEntity)
        {
            Library = library;
            Id = dataEntity.Id;
            //TODO other DataEntity fields
        }

        public override int Id { get; protected set; }
    }
}
