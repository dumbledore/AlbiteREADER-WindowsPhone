using SvetlinAnkov.Albite.Core.IO;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace SvetlinAnkov.Albite.Core.Serialization
{
    public class RecordStore
    {
        public string FilePath { get; private set; }

        private readonly string dbConnectionString;

        public RecordStore(string filePath)
        {
            FilePath = filePath;
            dbConnectionString = getConnectionString(filePath);
        }

        private static string getConnectionString(string location)
        {
            // See: http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202861(v=vs.105).aspx
            return string.Format("Data Source = 'isostore:/{0}';", location);
        }

        private RecordDataContext getDataContext(bool readOnly = false)
        {
            RecordDataContext db
                = new RecordDataContext(dbConnectionString, readOnly);

            if (!db.DatabaseExists())
            {
                // Make sure the path to the data base is there
                using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(FilePath))
                {
                    s.CreatePathForFile();
                }

                db.CreateDatabase();
            }

            return db;
        }

        private bool contains(string key, RecordDataContext db)
        {
            return db.Records.Count(r => r.Key == key) != 0;
        }

        public string this[string key]
        {
            get
            {
                using (RecordDataContext db = getDataContext(true))
                {
                    if (contains(key, db))
                    {
                        return db.Records.Single(r => r.Key == key).Value;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            set
            {
                using (RecordDataContext db = getDataContext())
                {
                    RecordEntity record;

                    if (contains(key, db))
                    {
                        // Get the entity if it's there
                        record = db.Records.Single(r => r.Key == key);

                        // And update it's value
                        record.Value = value;
                    }
                    else
                    {
                        // Create the entity if it does not exist
                        record = new RecordEntity(key, value);
                        db.Records.InsertOnSubmit(record);
                    }

                    // Update the DB
                    db.SubmitChanges();
                }
            }
        }

        [Table(Name="Records")]
        private class RecordEntity
        {
            // DataContext needs this, of course
            public RecordEntity() { }

            public RecordEntity(string key, string value)
            {
                Key = key;
                Value = value;
            }

            [Column(Name = "Key", IsPrimaryKey = true)]
            public string Key { get; private set; }

            [Column(Name = "Value")]
            public string Value { get; set; }
        }

        private class RecordDataContext : System.Data.Linq.DataContext
        {
#pragma warning disable 0649
            public Table<RecordEntity> Records;
#pragma warning restore 0649

            public RecordDataContext(string fileOrConnection, bool readOnly)
                : base(fileOrConnection)
            {
                ObjectTrackingEnabled = !readOnly;
            }
        }
    }
}
