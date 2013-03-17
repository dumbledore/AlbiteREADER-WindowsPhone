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
using SvetlinAnkov.Albite.Core.Test;

namespace SvetlinAnkov.Albite.Tests.Test.Model
{
    public abstract class LibraryTest : TestCase
    {
        private string location;

        public string DbPath { get; private set; }
        public string BooksPath { get; private set; }

        public LibraryTest(string location)
        {
            this.location = location;

            DbPath = location + "/Database.sdf";
            BooksPath = location + "/Books";
        }
    }
}
