using SvetlinAnkov.Albite.Core.Json;
using SvetlinAnkov.Albite.Core.Test;
using System;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.Core.Test
{
    public class JsonSerializerTest : TestCase
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
            JsonSerializer<Message> serializer
                = new JsonSerializer<Message>(expectedTypes);

            testMessage(serializer, new Message1(4, 16));
            testMessage(serializer, new Message2(64, 256));
            testMessage(serializer, new Message3("Sample data"));
            testMessage(serializer, new Message4(
                "Sample data",
                new Message4.Helper("snooker", 100, 147)
            ));
            testMessage(serializer, new Message4(
                null,
                new Message4.Helper("snooker", 100, 147)
            ));
            testMessage(serializer, new Message4(
                "Sample Data",
                null
            ));
            testMessage(serializer, null);
        }

        private void testMessage(
            JsonSerializer<Message> serializer, Message message)
        {
            string encoded = serializer.Encode(message);
            Log("Encoded {0}: {1}", message != null ? message.GetType().Name : "null", encoded);
            Message decoded = serializer.Decode(encoded);
            Log("{0} : {1}", decoded != null ? decoded.GetType().Name : "null", decoded);
        }

        [DataContract]
        private class Message { }

        [DataContract]
        private class Message1 : Message
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
        private class Message2 : Message
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
        private class Message3 : Message
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
        private class Message4 : Message
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
