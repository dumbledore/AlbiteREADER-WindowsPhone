using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.Serialization;
using SvetlinAnkov.Albite.Engine.Layout;
using SvetlinAnkov.Albite.READER.View.Pages.BookSettings;

namespace SvetlinAnkov.Albite.READER
{
    public class AlbiteContext
    {
        public Library Library { get; private set; }
        public RecordStore RecordStore { get; private set; }

        public AlbiteContext(string libraryPath, string recordStorePath)
        {
            Library = new BookLibrary.Library(libraryPath);
            RecordStore = new RecordStore(recordStorePath);
        }

        public static readonly string LayoutSettingsKey = "layout-settings";

        private LayoutSettings cachedSettings = null;
        public LayoutSettings LayoutSettings
        {
            get
            {
                if (cachedSettings == null)
                {
                    if (RecordStore.ContainsKey(LayoutSettingsKey))
                    {
                        // Get data
                        string s = RecordStore[LayoutSettingsKey];

                        // Unserialize
                        cachedSettings = LayoutSettings.FromString(s);
                    }
                    else
                    {
                        // Default Settings
                        cachedSettings = getDefaultSettings();

                        // No need to persist them as
                        // they would be created again
                        // next time if they are not changed
                        // and therefore not persisted
                    }
                }

                return cachedSettings;
            }

            set
            {
                // Cache
                cachedSettings = value;

                // Persist
                RecordStore[LayoutSettingsKey] = value.ToString();
            }
        }

        private LayoutSettings getDefaultSettings()
        {
            LayoutSettings settings = DefaultLayoutSettings.LayoutSettings;

            // TODO Fill this in appropriately

            return settings;
        }
    }
}
