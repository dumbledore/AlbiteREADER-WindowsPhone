using SvetlinAnkov.Albite.Core.Serialization;
using System;
using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Core.Json
{
    public class JsonMessenger<THandler> : AlbiteMessenger<THandler>
    {
        public JsonMessenger(THandler handler, IClientNotifier clientNotifier, IEnumerable<Type> messageTypes)
            : base(
                handler, clientNotifier,
                new JsonSerializer<JsonMessenger<THandler>.AlbiteMessage>(messageTypes)
            ) { }
    }
}
