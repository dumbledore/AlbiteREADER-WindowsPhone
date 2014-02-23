using Albite.Reader.Core.Serialization;
using System;
using System.Collections.Generic;

namespace Albite.Reader.Core.Json
{
    public class JsonMessenger<THandler> : Messenger<THandler>
    {
        public JsonMessenger(THandler handler, IClientNotifier clientNotifier, IEnumerable<Type> messageTypes)
            : base(
                handler, clientNotifier,
                new JsonSerializer<JsonMessenger<THandler>.Message>(messageTypes)
            ) { }
    }
}
