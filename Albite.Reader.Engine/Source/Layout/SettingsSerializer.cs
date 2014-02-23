using Albite.Reader.Core.Json;
using Albite.Reader.Core.Serialization;
using System;
using System.Windows.Media;

namespace Albite.Reader.Engine.Layout
{
    internal class SettingsSerializer : ISerializer<object>
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