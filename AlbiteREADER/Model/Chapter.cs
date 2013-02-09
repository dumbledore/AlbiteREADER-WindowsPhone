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
using SvetlinAnkov.Albite.READER.Layout;

namespace SvetlinAnkov.Albite.READER.Model
{
    public class Chapter
    {
        public Chapter PreviousChapter { get; private set; }
        public Chapter NextChapter {get; private set; }
        public Book Book { get; private set; }
        public Location ReadingLocation { get; set; }

        public Chapter(Book book, Chapter previous, Chapter next)
        {
            Book = book;
            PreviousChapter = previous;
            NextChapter = next;
        }
    }
}
