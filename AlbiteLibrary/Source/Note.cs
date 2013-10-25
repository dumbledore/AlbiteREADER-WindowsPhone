using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Data.Linq;
using SvetlinAnkov.Albite.BookLibrary.DataContext;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class Note : LibraryEntity
    {
        public NoteManager Manager { get; private set; }

        public BookPresenter.Location Location { get; private set; }
        public string Text { get; private set; }

        internal Note(NoteManager manager, NoteEntity entity)
            : base(manager.Library, entity)
        {
            Manager = manager;

            // Other fields
            Location = manager.BookPresenter.CreateLocation(
                entity.SpineIndex,
                entity.DomLocation,
                entity.TextLocation);

            Text = entity.Text;
        }
    }
}
