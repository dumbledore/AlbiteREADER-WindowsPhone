using Microsoft.Phone.Shell;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace SvetlinAnkov.Albite.READER
{
    public static class TileManager
    {
        private const string pageNameQuery = "ReaderPage.xaml?id=";
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

        private static Uri DefaultCover = new Uri("Resources/Book.png", UriKind.Relative);

        private static Uri makeCover(BookPresenter bookPresenter)
        {
            Uri coverUri = DefaultCover;

            using (Stream coverStream = bookPresenter.GetCoverStream())
            {
                // Is there a cover?
                if (coverStream != null)
                {
                    try
                    {
                        // New bitmap image
                        BitmapImage bmp = new BitmapImage();

                        // Use the cover stream. Would it work with non-JPEG images?
                        bmp.SetSource(coverStream);

                        // Make a new WriteableBitmap so we could save it
                        WriteableBitmap wbmp = new WriteableBitmap(bmp);

                        // Crop the bitmap
                        wbmp = crop(wbmp);

                        // Output name
                        string outputName = getTilePath(bookPresenter.Book);

                        // Cover uri
                        coverUri = new Uri("isostore:/" + outputName, UriKind.Absolute);

                        using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(outputName))
                        {
                            using (Stream outputStream = iso.GetStream(FileAccess.Write))
                            {
                                // Save & resize
                                wbmp.SaveJpeg(outputStream, 173, 173, 0, 95);
                            }
                        }
                    }
                    catch { }
                }
            }

            return coverUri;
        }

        private static string getTilePath(Book book)
        {
            return string.Format("Shared/ShellContent/book-{0}.jpg", book.Id);
        }

        private static WriteableBitmap crop(WriteableBitmap source)
        {
            int srcWidth = source.PixelWidth;
            int srcHeight = source.PixelHeight;

            int size = Math.Min(srcWidth, srcHeight);

            int width = size;
            int height = size;

            int x = (srcWidth - size) / 2;
            int y = (srcHeight - size) / 2;

            // Clamp to boundaries
            if (x < 0) x = 0;
            if (x + width > srcWidth) width = srcWidth - x;
            if (y < 0) y = 0;
            if (y + height > srcHeight) height = srcHeight - y;

            // Copy the pixels line by line using fast BlockCopy
            WriteableBitmap dest = new WriteableBitmap(size, size);

            for (var line = 0; line < height; line++)
            {
                // RGBA = 4 bytes
                int srcOff = ((y + line) * srcWidth + x) * 4;
                int dstOff = line * width * 4;

                Buffer.BlockCopy(source.Pixels, srcOff, dest.Pixels, dstOff, width * 4);
            }

            return dest;
        }
    }
}
