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
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.READER.Model.Container;
using System.Linq;
using System.Data.Linq.Mapping;

namespace SvetlinAnkov.Albite.READER.Model
{
    [Table(Name = "Books")]
    public class Book
    {
        public static string IsoLocationPath { get { return "Books/"; } }

        // ID
        private int id;

        [Column(Storage = "id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        public int Id
        {
            get { return id; }
            private set { id = value; }
        }

        // Title (string)
        public string Title { get; set; }

        // Author (reference)

        // Genre (reference)

        // Subjects (reference)

        // Download URL so that one would be able to
        // easily download the contents again.
        //
        // This might include some special protocol name
        // for non http links, e.g.:
        //
        //   myuser@dropbox://mybooks/book.epub
        //   myuser@skydrive://mybooks/book.epub

        // Archived (boolean)

        // Reading Persistance (lazy initialisation)
        // Last Chapter (int)
        // Position in Chater (string)
        // Chapter Bookmarks, Highlights and Notes
    }
}
