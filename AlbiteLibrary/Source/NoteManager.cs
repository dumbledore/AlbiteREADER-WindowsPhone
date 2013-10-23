﻿using SvetlinAnkov.Albite.Library.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library
{
    public class NoteManager : EntityManager<Note>
    {
        public BookPresenter BookPresenter { get; private set; }

        internal NoteManager(BookPresenter bookPresenter)
            : base(bookPresenter.Book.Library)
        {
            BookPresenter = bookPresenter;
        }

        public Note Add(BookPresenter.Location location, string text)
        {
            using (LibraryDataContext dc = Library.GetDataContext())
            {
                BookEntity bookEntity = getBookEntity(dc);
                NoteEntity noteEntity = new NoteEntity(bookEntity, location, text);
                bookEntity.Notes.Add(noteEntity);
                dc.SubmitChanges();
                return new Note(this, noteEntity);
            }
        }

        public override void Remove(Note entity)
        {
            throw new NotImplementedException();
        }

        public override Note this[int id]
        {
            get
            {
                using (LibraryDataContext dc = Library.GetDataContext())
                {
                    NoteEntity noteEntity = dc.Notes.Single(
                        n => n.Id == id && n.Book.Id == BookPresenter.Book.Id);
                    return new Note(this, noteEntity);
                }
            }
        }

        public override IList<Note> GetAll()
        {
            using (LibraryDataContext dc = Library.GetDataContext())
            {
                BookEntity bookEntity = getBookEntity(dc);
                EntitySet<NoteEntity> noteSet = bookEntity.Notes;

                int count = noteSet.Count();
                List<Note> notes = new List<Note>(count);

                foreach (NoteEntity noteEntity in noteSet)
                {
                    notes.Add(new Note(this, noteEntity));
                }

                return notes.ToArray();
            }
        }

        private BookEntity getBookEntity(LibraryDataContext dc)
        {
            return BookManager.GetEntity(dc, BookPresenter.Book.Id);
        }
    }
}
