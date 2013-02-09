﻿using System;
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

namespace SvetlinAnkov.Albite.READER.View
{
    public partial class BookPage : PhoneApplicationPage
    {
        public BookPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine("Size changed: {0}x{1}", e.NewSize.Width, e.NewSize.Height);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Debug.WriteLine("Navigated to page. Query: {0}", e.Uri.Query);
        }
    }
}