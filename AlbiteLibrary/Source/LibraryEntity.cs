namespace SvetlinAnkov.Albite.BookLibrary
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
