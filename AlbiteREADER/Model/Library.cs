using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace SvetlinAnkov.Albite.READER.Model
{
    //TODO: Perhaps this class will be used for different purposes

    public class Library
    {
        private Dictionary<int, Book> cachedBooks = new Dictionary<int, Book>();
        private Object myLock = new Object();
        private int nextBookId = 0;

        private Book LoadFromStorage(int id)
        {
            //TODO: SQL-based
            return null;
        }

        private void SaveToStorage(Book book)
        {
            //TODO
        }

        public Book this[int id]
        {
            get
            {
                lock(myLock)
                {
                    // Look in cached books
                    if (cachedBooks.ContainsKey(id))
                    {
                        return cachedBooks[id];
                    }

                    // try load from DB
                    Book book = LoadFromStorage(id);
                    if (book != null)
                    {
                        // cache it then
                        cachedBooks[id] = book;
                    }

                    return book;
                }
            }
        }

        //public void addBook(BookContainer bookContainer)
        //{
        //    Book book = new Book();
        //}
    }
}
