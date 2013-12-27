using SvetlinAnkov.Albite.Core.Json;
using SvetlinAnkov.Albite.Core.Serialization;
using System;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.Engine.Layout
{
    internal class SettingsSerializer : IAlbiteSerializer<object>
    {
        private static readonly Type[] expectedTypes = new Type[]
        {
            typeof(FontSettings),
            typeof(FontSize),
            typeof(LayoutSettings),
            typeof(LineHeight),
            typeof(MarginSettings),
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