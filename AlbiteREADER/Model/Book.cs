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
using SvetlinAnkov.AlbiteREADER.Utils;
using SvetlinAnkov.AlbiteREADER.Model.Container;

namespace SvetlinAnkov.AlbiteREADER.Model
{
    public class Book
    {
        private const string IsoLocationPath = "Books/";

        //TODO: This will be set by the DataBase
        private int id = 0;

        public Book(BookContainer bookContainer)
        {
            // Create the folder for this book
            
            // Extract the 
        }

        public virtual string Path
        {
            get
            {
                return Defaults.Engine.BookPath + id;
            }
        }
    }
}
