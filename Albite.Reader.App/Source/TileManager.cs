﻿using Microsoft.Phone.Shell;
using Albite.Reader.BookLibrary;
using Albite.Reader.Core.IO;
using Albite.Reader.App.View.Controls;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Albite.Reader.Core.Media;

namespace Albite.Reader.App
{
    public static class TileManager
    {
        private const string pageNameQuery = "BooksPage.xaml?id=";
        private const string pagePath = "/Albite.Reader.App;component/Source/View/Pages/";

        public static bool PinBook(BookPresenter bookPresenter)
        {
            bool addedNew = false;

            ShellTile tile = getTile(bookPresenter.Book);

            if (tile == null)
            {
                // Set up tile data
                StandardTileData tileData = new StandardTileData();
                tileData.BackgroundImage = makeCover(bookPresenter);
                tileData.BackTitle = bookPresenter.Book.Author;
                tileData.BackContent = bookPresenter.Book.Title;
                tileData.BackBackgroundImage = BackTileImage;

                // Create the navigation URI
                Uri navigationUri = new Uri(
                    pagePath + pageNameQuery + bookPresenter.Book.Id.ToString(), UriKind.Relative);

                // Create the tile
                ShellTile.Create(navigationUri, tileData);

                addedNew = true;
            }

            return addedNew;
        }

        public static bool UnpinBook(Book book)
        {
            bool removed = false;

            ShellTile tile = getTile(book);

            if (tile != null)
            {
                tile.Delete();

                // Try removing the tile background if there was one
                try
                {
                    string tilePath = getTilePath(book);

                    using (IsolatedStorage iso = new IsolatedStorage(tilePath))
                    {
                        iso.Delete();
                    }
                }
                catch { }

                removed = true;
            }

            return removed;
        }

        public static bool IsPinned(Book book)
        {
            ShellTile tile = getTile(book);
            return tile != null;
        }

        private static ShellTile getTile(Book book)
        {
            // Get the search string ready
            string pageNameSearch = pageNameQuery + book.Id.ToString();

            // Search for this string in available tiles
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(
                x => x.NavigationUri.ToString().Contains(pageNameSearch));

            return tile;
        }

        private static readonly Uri DefaultCover = new Uri("Resources/Book.png", UriKind.Relative);

        private static readonly Uri BackTileImage = new Uri("Resources/BackTile.png", UriKind.Relative);

        private static readonly int TileSize = 336;

        private static Uri makeCover(BookPresenter bookPresenter)
        {
            Uri coverUri = DefaultCover;

            using (Stream coverStream = bookPresenter.GetCoverStream())
            {
                try
                {
                    FrameworkElement coverElement;

                    // Is there a cover?
                    if (coverStream != null)
                    {
                        // New bitmap image
                        BitmapImage bmp = new BitmapImage();

                        // Use the cover stream. Would it work with non-JPEG images?
                        bmp.SetSource(coverStream);

                        // Create a new image control
                        Image image = new Image();

                        // Uniform stretch, so that the whole book cover
                        // would be visible, without being stretched
                        image.Stretch = Stretch.Uniform;

                        // Set the bitmap image to the image control
                        image.Source = bmp;

                        // Set up its dimensions
                        image.Width = TileSize;
                        image.Height = TileSize;

                        // Set the cover element
                        coverElement = image;
                    }
                    else
                    {
                        // Create a new BookTileControl
                        BookTileControl bookTile = new BookTileControl();

                        // Create a random color for the book cover
                        bookTile.BookColor = createRandomColor();

                        // Set the title of the book cover
                        bookTile.BookTitle = bookPresenter.Book.Title;

                        // Set up its dimensions
                        bookTile.Measure(new Size(TileSize, TileSize));
                        bookTile.Arrange(new Rect(0, 0, TileSize, TileSize));

                        // Set the cover element
                        coverElement = bookTile;
                    }


                    // Make a new WriteableBitmap from the image control,
                    // so we could save it. No transform is needed
                    WriteableBitmap wbmp = new WriteableBitmap(coverElement, null);

                    // Output name
                    string outputName = getTilePath(bookPresenter.Book);

                    using (IsolatedStorage iso = new IsolatedStorage(outputName))
                    {
                        using (Stream outputStream = iso.GetStream(FileAccess.Write))
                        {
                            // Save & resize
                            wbmp.SaveJpeg(outputStream, TileSize, TileSize, 0, 95);
                        }
                    }

                    // Set cover uri only after it successfully created the pin image
                    coverUri = new Uri("isostore:/" + outputName, UriKind.Absolute);
                }
                catch { }
            }

            return coverUri;
        }

        // The random needs to be created once,
        // otherwise it would not be random at all!
        private static Random random = new Random();

        private static Color createRandomColor()
        {
            int hue = random.Next(360);
            HslColor hslColor = new HslColor(hue, 1.0, 0.3333);
            return hslColor.ToColor();
        }

        private static string getTilePath(Book book)
        {
            return string.Format("Shared/ShellContent/book-{0}.jpg", book.Id);
        }
    }
}
