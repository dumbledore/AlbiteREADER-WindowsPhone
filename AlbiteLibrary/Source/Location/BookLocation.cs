using SvetlinAnkov.Albite.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "bookLocation")]
    public class BookLocation : IComparable<BookLocation>, IContextAttachable<BookPresenter>
    {
        private BookPresenter context_;

        public bool IsAttached
        {
            get { return context_ != null; }
        }

        public BookPresenter Context
        {
            get { return context_; }
        }

        public void Attach(BookPresenter context)
        {
            if (IsAttached)
            {
                // Can't reattach
                throw new InvalidOperationException("Already attached");
            }

            // Validate the context
            if (bookId != context.Book.Id                   // different book
                || chapterNumber < 0                        // invalid chapter number
                || context.Spine.Length <= chapterNumber)   // chapter is out of range
            {
                throw new InvalidOperationException("Invalid context");
            }

            // All set
            context_ = context;
        }

        private void throwIfNotAttached()
        {
            if (!IsAttached)
            {
                throw new InvalidOperationException("Not attached");
            }
        }

        [DataMember(Name = "bookId")]
        private int bookId;

        [DataMember(Name = "chapterNumber")]
        private int chapterNumber;

        public Chapter Chapter
        {
            get
            {
                // Validate state
                throwIfNotAttached();

                // return dynamically
                return context_.Spine[chapterNumber];
            }
        }

        [DataMember(Name = "domLocation")]
        private DomLocation domLocation_;

        public DomLocation DomLocation
        {
            get
            {
                // Validate state
                throwIfNotAttached();

                return domLocation_;
            }
        }

        internal BookLocation(
            Chapter chapter,
            DomLocation domLocation)
        {
            // Set up the context
            context_ = chapter.BookPresenter;
            bookId = context_.Book.Id;

            // Set the data
            chapterNumber = chapter.Number;
            domLocation_ = domLocation;
        }

        public int CompareTo(BookLocation other)
        {
            // Validate state
            throwIfNotAttached();

            int thisSpineIndex = Chapter.Number;
            int otherSpineIndex = other.Chapter.Number;

            if (thisSpineIndex != otherSpineIndex)
            {
                return thisSpineIndex < otherSpineIndex ? -1 : 1;
            }

            return DomLocation.CompareTo(other.DomLocation);
        }

        public static BookLocation FromString(string encodedData)
        {
            if (encodedData == null)
            {
                throw new ArgumentException();
            }

            LibrarySerializer serializer = new LibrarySerializer();
            return (BookLocation)serializer.Decode(encodedData);
        }

        public override string ToString()
        {
            LibrarySerializer serializer = new LibrarySerializer();
            return serializer.Encode(this);
        }
    }
}
