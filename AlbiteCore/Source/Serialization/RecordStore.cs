using SvetlinAnkov.Albite.Core.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace SvetlinAnkov.Albite.Core.Serialization
{
    public class RecordStore : IDictionary<string, string>
    {
        public string FilePath { get; private set; }

        private readonly string dbConnectionString;

        public RecordStore(string filePath)
        {
            FilePath = filePath;
            dbConnectionString = getConnectionString(filePath);
        }

#region Helpers
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

        private RecordEntity getEntity(string key, RecordDataContext db)
        {
            return db.Records.Single(r => r.Key == key);
        }
#endregion

#region IDictionary<string, string>
        private void throwIfKeyIsNull(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key is null");
            }
        }

        private bool containsKey_(string key, RecordDataContext db)
        {
            return db.Records.Count(r => r.Key == key) != 0;
        }

        public ICollection<string> Keys
        {
            get
            {
                using (RecordDataContext db = getDataContext(true))
                {
                    return (from r in db.Records select r.Key).ToArray();
                }
            }
        }

        public ICollection<string> Values
        {
            get
            {
                using (RecordDataContext db = getDataContext(true))
                {
                    return (from r in db.Records select r.Value).ToArray();
                }
            }
        }

        public string this[string key]
        {
            get
            {
                throwIfKeyIsNull(key);

                using (RecordDataContext db = getDataContext(true))
                {
                    if (!containsKey_(key, db))
                    {
                        throw new KeyNotFoundException("key not found");
                    }

                    return getEntity(key, db).Value;
                }
            }

            set
            {
                throwIfKeyIsNull(key);

                using (RecordDataContext db = getDataContext())
                {
                    RecordEntity record;

                    if (containsKey_(key, db))
                    {
                        // Get the entity if it's there
                        record = getEntity(key, db);

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

        public void Add(string key, string value)
        {
            throwIfKeyIsNull(key);

            using (RecordDataContext db = getDataContext())
            {
                if (containsKey_(key, db))
                {
                    throw new ArgumentException("An element with the same key already exists");
                }
            }
        }

        public bool ContainsKey(string key)
        {
            throwIfKeyIsNull(key);

            using (RecordDataContext db = getDataContext(true))
            {
                return containsKey_(key, db);
            }
        }

        public bool Remove(string key)
        {
            throwIfKeyIsNull(key);

            using (RecordDataContext db = getDataContext())
            {
                if (!containsKey_(key, db))
                {
                    return false;
                }

                try
                {
                    RecordEntity record = getEntity(key, db);
                    db.Records.DeleteOnSubmit(record);
                    db.SubmitChanges();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryGetValue(string key, out string value)
        {
            throwIfKeyIsNull(key);

            using (RecordDataContext db = getDataContext(true))
            {
                if (containsKey_(key, db))
                {
                    value = this[key];
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
        }
#endregion

#region ICollection<KeyValuePair<string, string>>
        public int Count
        {
            get
            {
                using (RecordDataContext db = getDataContext(true))
                {
                    return db.Records.Count();
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            using (RecordDataContext db = getDataContext())
            {
                db.Records.DeleteAllOnSubmit(db.Records);
            }
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            using (RecordDataContext db = getDataContext(true))
            {
                if (containsKey_(item.Key, db))
                {
                    return this[item.Key] == item.Value;
                }

                return false;
            }
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new NullReferenceException("array is null");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex is less than 0");
            }

            using (RecordDataContext db = getDataContext(true))
            {
                // Get the count
                int count = db.Records.Count();

                // Validate the array would be able to hold the data
                if (arrayIndex + count > array.Length)
                {
                    throw new ArgumentException("Not enough space in array");
                }

                // Now copy to output
                int i = arrayIndex;
                foreach (RecordEntity record in db.Records)
                {
                    array[i++] = new KeyValuePair<string, string>(record.Key, record.Value);
                }
            }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            using (RecordDataContext db = getDataContext())
            {
                if (containsKey_(item.Key, db))
                {
                    if (this[item.Key] == item.Value)
                    {
                        return Remove(item.Key);
                    }
                }

                return false;
            }
        }
#endregion

#region IEnumerator<KeyValuePair<string, string>>
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            throw new NotSupportedException();
        }

#endregion

#region IEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }
#endregion

#region RecordEntity
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
#endregion

#region RecordDataContext
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
#endregion

    }
}
