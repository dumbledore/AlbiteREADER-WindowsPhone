﻿using System;
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

        private static CachedResourceImage cachedFolderImage
            = new CachedResourceImage("/Resources/Images/folder.png");

        private static CachedResourceImage cachedFolderImageDark
            = new CachedResourceImage("/Resources/Images/folder-dark.png");

        private static void onFolderItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FolderControl control = (FolderControl)d;

            IFolderItem newValue = (IFolderItem)e.NewValue;

            control.ContentTextBlock.Text = newValue.Name;

            control.Icon.Source = newValue.IsFolder
                ? (ThemeInfo.ThemeIsDark ? cachedFolderImageDark.Value : cachedFolderImage.Value)
                : newValue.FileIcon;
        }
    }
}