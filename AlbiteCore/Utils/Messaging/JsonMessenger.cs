using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace SvetlinAnkov.Albite.Core.Utils.Messaging
{
    public class JsonMessenger
    {
        private readonly DataContractJsonSerializer serializer;

        public JsonMessenger(IEnumerable<Type> messageTypes)
        {
            serializer = new DataContractJsonSerializer(
                typeof(JsonMessage), messageTypes);
        }

        public string Encode(JsonMessage message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, message);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public JsonMessage Decode(string data)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                JsonMessage message = (JsonMessage)serializer.ReadObject(stream);
                return (JsonMessage)message;
            }
        }

        [DataContract]
        public class JsonMessage
        {
            public virtual void Callback() { }
        }
    }
}
