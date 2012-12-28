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

namespace SvetlinAnkov.AlbiteREADER.Model
{
    public class Book
    {
        private const string IsoLocationPath = "Books/";

        private int id = 0;

        public virtual string IsoLocation
        {
            get
            {
                return IsoLocationPath + id;
            }
        }
    }
}
