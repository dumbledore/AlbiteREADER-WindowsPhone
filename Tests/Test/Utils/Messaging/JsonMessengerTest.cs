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
            Type[] expectedTypes = new Type[] {
                typeof(Message1),
                typeof(Message2),
                typeof(Message3),
                typeof(Message4),
            };
            // We want the client/host messages to be the same
            JsonMessenger messenger = new JsonMessenger(expectedTypes);

            testMessage(messenger, new Message1(4, 16));
            testMessage(messenger, new Message2(64, 256));
            testMessage(messenger, new Message3("Sample data"));
            testMessage(messenger, new Message4(
                "Sample data",
                new Message4.Helper("snooker", 100, 147)
            ));
            testMessage(messenger, new Message4(
                null,
                new Message4.Helper("snooker", 100, 147)
            ));
            testMessage(messenger, new Message4(
                "Sample Data",
                null
            ));
            testMessage(messenger, null);
        }

        private void testMessage(
            JsonMessenger messenger, JsonMessenger.JsonMessage message)
        {
            string encoded = messenger.Encode(message);
            Log("Encoded {0}: {1}", message != null ? message.GetType().Name : "null", encoded);
            JsonMessenger.JsonMessage decoded = messenger.Decode(encoded);
            Log("{0} : {1}", decoded != null ? decoded.GetType().Name : "null", decoded);
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

            public override string ToString()
            {
                return string.Format(
                    "{{ info1 : {0}, info2 : {1} }}",
                    Info1, Info2
                );
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

            public override string ToString()
            {
                return string.Format(
                    "{{ info1 : {0}, info2 : {1} }}",
                    Info1, Info2
                );
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

            public override string ToString()
            {
                return string.Format("{{ data : {0} }}", Data);
            }
        }

        [DataContract]
        private class Message4 : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "data")]
            public string Data { get; private set; }

            [DataMember(Name = "extra")]
            public Helper Extra { get; private set; }

            public Message4(string data, Helper extra)
            {
                Data = data;
                Extra = extra;
            }

            public override string ToString()
            {
                return string.Format(
                    "Message4 : {{ data : {0}, extra : {1} }}",
                    Data, Extra
                );
            }

            [DataContract]
            public class Helper
            {
                [DataMember(Name = "data")]
                public string Data { get; private set; }

                [DataMember(Name = "info1")]
                public int Info1 { get; private set; }

                [DataMember(Name = "info2")]
                public int Info2 { get; private set; }

                public Helper(string data, int info1, int info2)
                {
                    Data = data;
                    Info1 = info1;
                    Info2 = info2;
                }

                public override string ToString()
                {
                    return string.Format(
                        "{{ data : {0}, info1 : {1}, info2 : {2} }}",
                        Data, Info1, Info2
                    );
                }
            }
        }
    }
}
