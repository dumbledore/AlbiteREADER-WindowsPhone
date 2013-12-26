using SvetlinAnkov.Albite.Core.Json;
using SvetlinAnkov.Albite.Core.Serialization;
using System;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    internal class SettingsSerializer : IAlbiteSerializer<object>
    {
        private static readonly Type[] expectedTypes = new Type[]
        {
            typeof(FontSettings),
            typeof(MarginSettings),
            typeof(Settings),
            typeof(TextSettings),
            typeof(Theme),
            typeof(Color),
        };

        private readonly JsonSerializer<object> serializer;

        public SettingsSerializer()
        {
            serializer = new JsonSerializer<object>(expectedTypes);
        }

        public string Encode(object entity)
        {
            return serializer.Encode(entity);
        }

        public object Decode(string data)
        {
            return serializer.Decode(data);
        }
    }
}