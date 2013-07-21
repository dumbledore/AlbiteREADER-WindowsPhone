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
        private readonly DataContractJsonSerializer clientSerializer;
        private readonly DataContractJsonSerializer hostSerializer;

        public JsonMessenger(
            IEnumerable<Type> clientMessages,
            IEnumerable<Type> hostMessages)
        {
            clientSerializer = new DataContractJsonSerializer(
                typeof(JsonMessage), clientMessages);

            hostSerializer = new DataContractJsonSerializer(
                typeof(JsonMessage), hostMessages);
        }

        public string Encode(JsonMessage message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                hostSerializer.WriteObject(stream, message);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public JsonMessage Decode(string data)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                JsonMessage message = (JsonMessage)clientSerializer.ReadObject(stream);
                return (JsonMessage)message;
            }
        }

        [DataContract]
        public class JsonMessage { }
    }
}
