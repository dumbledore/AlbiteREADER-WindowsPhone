using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Albite.Reader.App.Browse;

namespace Albite.Reader.App.View.Controls
{
    public class FolderControl : IconContentControl
    {
        public static readonly DependencyProperty FolderItemProperty
            = DependencyProperty.Register("FolderItem", typeof(IFolderItem), typeof(FolderControl),
            new PropertyMetadata(onFolderItemChanged));

        public IFolderItem FolderItem
        {
            get { return (IFolderItem)GetValue(FolderItemProperty); }
            set { SetValue(FolderItemProperty, value); }
        }

        private static void onFolderItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FolderControl control = (FolderControl)d;

            IFolderItem newValue = (IFolderItem)e.NewValue;

            //// Set up book position as header text
            //control.HeaderText = string.Format("{0:P0}", getReadingPosition(newValue));

            //// Set up bookmark text as content text
            //control.ContentText = newValue.Text;
        }
    }
}
