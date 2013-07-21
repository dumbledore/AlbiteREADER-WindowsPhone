using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Core.Utils.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace SvetlinAnkov.Albite.Tests.Test.Utils.Messaging
{
    class JsonMessengerTest : TestCase
    {
        protected override void TestImplementation()
        {
            // We want the client/host messages to be the same
            JsonMessenger messenger = new JsonMessenger(
                new Type[] { typeof(Message1), typeof(Message2), typeof(Message3) },
                new Type[] { typeof(Message1), typeof(Message2), typeof(Message3) }
            );

            Message1 msg1Original = new Message1(4, 16);
            string msg1String = messenger.Encode(msg1Original);
            Log("Message1 : {0}", msg1String);
            Message1 msg1Deserialized = (Message1)messenger.Decode(msg1String);
            Log("Message1 : {{ info1 : {0}, info2 : {1} }}",
                msg1Deserialized.Info1, msg1Deserialized.Info2);

            Message2 msg2Original = new Message2(64, 256);
            string msg2String = messenger.Encode(msg2Original);
            Log("Message2 : {0}", msg2String);
            Message2 msg2Deserialized = (Message2)messenger.Decode(msg2String);
            Log("Message2 : {{ info1 : {0}, info2 : {1} }}",
                msg2Deserialized.Info1, msg2Deserialized.Info2);

            Message3 msg3Original = new Message3("Sample data");
            string msg3String = messenger.Encode(msg3Original);
            Log("Message3 : {0}", msg3String);
            Message3 msg3Deserialized = (Message3)messenger.Decode(msg3String);
            Log("Message3 : {{ data : {0} }}",
                msg3Deserialized.Data);
        }

        [DataContract]
        private class Message1 : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "info1")]
            public int Info1 { get; private set; }

            [DataMember(Name = "info2")]
            public int Info2 { get; private set; }

            public Message1(int info1, int info2)
            {
                Info1 = info1;
                Info2 = info2;
            }
        }

        [DataContract]
        private class Message2 : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "info1")]
            public int Info1 { get; private set; }

            [DataMember(Name = "info2")]
            public int Info2 { get; private set; }

            public Message2(int info1, int info2)
            {
                Info1 = info1;
                Info2 = info2;
            }
        }

        [DataContract]
        private class Message3 : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "data")]
            public string Data { get; private set; }

            public Message3(string data)
            {
                Data = data;
            }
        }
    }
}
