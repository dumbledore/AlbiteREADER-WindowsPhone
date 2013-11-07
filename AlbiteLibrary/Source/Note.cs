using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.BookLibrary.Location;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class Note : LibraryEntity
    {
        public NoteManager Manager { get; private set; }

        public BookLocation BookLocation { get; private set; }
        public string Text { get; private set; }

        internal Note(NoteManager manager, NoteEntity entity)
            : base(manager.Library, entity)
        {
            Manager = manager;

            // Other fields
            BookLocation = manager.BookPresenter.CreateLocation(
                entity.SpineIndex,
                entity.DomLocation);

            Text = entity.Text;
        }
    }
}
