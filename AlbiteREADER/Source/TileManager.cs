using Microsoft.Phone.Shell;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.READER.View.Controls;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SvetlinAnkov.Albite.READER
{
    public static class TileManager
    {
        private const string pageNameQuery = "BooksPage.xaml?id=";
        private const string pagePath = "/AlbiteREADER;component/Source/View/Pages/";

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

                    using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(tilePath))
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

        private static readonly int TileSize = 173;

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

                    using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(outputName))
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

        private static Color createRandomColor()
        {
            Random random = new Random();

            int color = random.Next();

            byte r = (byte)((color >>  0) & 0xFF);
            byte g = (byte)((color >>  8) & 0xFF);
            byte b = (byte)((color >> 16) & 0xFF);

            return Color.FromArgb(0xFF, r, g, b);
        }

        private static string getTilePath(Book book)
        {
            return string.Format("Shared/ShellContent/book-{0}.jpg", book.Id);
        }
    }
}
