using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Diagnostics.CodeAnalysis;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.Library
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
        private LibraryDataContext db;
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
        internal LibraryDataContext GetDataContext()
        {
            LibraryDataContext db
                = new LibraryDataContext(dbConnectionString);

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
            return string.Format(
                "Data Source = 'isostore:/{0}'; Max Database Size = '{1}';",
                location, maxSize);
        }
    }
}
