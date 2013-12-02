using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.Core.IO;
using System.IO;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class Library
    {
        // Public API
        public BookManager Books { get; private set; }
        // TODO:
        // Authors
        // Genres
        // Subjects

        public string LibraryPath { get; private set; }

        // Private Implementation
        private string dbPath;
        private string dbConnectionString;

        public Library(string libraryPath)
        {
            // Cache the paths
            LibraryPath = libraryPath;
            dbPath = Path.Combine(LibraryPath, "Database.sdf");
            dbConnectionString = getConnectionString(dbPath);

            // Create the top-level managers
            Books = new BookManager(this);
        }

        /// <summary>
        /// Creates a new LibraryDataContext
        /// </summary>
        /// <returns>Returns a new LibraryDataContext</returns>
        internal LibraryDataContext GetDataContext(bool readOnly = false)
        {
            LibraryDataContext db
                = new LibraryDataContext(dbConnectionString, readOnly);

            if (!db.DatabaseExists())
            {
                // Make sure the path to the data base is there
                using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(dbPath))
                {
                    s.CreatePathForFile();
                }

                db.CreateDatabase();
            }

            return db;
        }

        private static string getConnectionString(string location, int maxSize = 128)
        {
            // The size is in MB
            // See: http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202861(v=vs.105).aspx
            return string.Format(
                "Data Source = 'isostore:/{0}'; Max Database Size = '{1}';",
                location, maxSize);
        }
    }
}
