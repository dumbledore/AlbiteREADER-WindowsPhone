using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.READER.Model.Reader;
using SvetlinAnkov.Albite.READER.Utils;

namespace SvetlinAnkov.Albite.READER.View
{
    public partial class ReaderPage : PhoneApplicationPage
    {
        private int bookId;

        public ReaderPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) { }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the book id from the query string
            bookId = int.Parse(NavigationContext.QueryString["id"]);
        }

        private void ReaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReaderControl.OpenBook(bookId);
        }

        // TODO: Add proper thread synchronization
    }
}