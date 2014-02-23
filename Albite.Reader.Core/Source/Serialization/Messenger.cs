using System.Runtime.Serialization;
namespace Albite.Reader.Core.Serialization
{
    public abstract class Messenger<THandler>
    {
        private Serializer<Message> serializer;
        private THandler handler;
        private IClientNotifier clientNotifier;

        public Messenger(
            THandler handler,
            IClientNotifier clientNotifier,
            Serializer<Message> serializer)
        {
            this.serializer = serializer;
            this.handler = handler;
            this.clientNotifier = clientNotifier;
        }

        /// <summary>
        /// Receives a message from the client and passes the
        /// handler to its callback
        /// </summary>
        /// <param name="serializedMessage">Serialized message</param>
        public void NotifyHost(string serializedMessage)
        {
            // Receiving a notification from client
            Message message = serializer.Decode(serializedMessage);

            // Running the callback will make sure
            // the message does its job
            message.Callback(handler);
        }

        /// <summary>
        /// Encodes a message and passes it to the client.
        /// Decodes the result, executes its callback and returns the message.
        /// Used by implementations.
        /// </summary>
        /// <param name="message">The result of from the client</param>
        /// <returns></returns>
        protected Message NotifyClient(Message message)
        {
            // Encode the message
            string encodedMessage = serializer.Encode(message);

            // Send to the client and get the encoded result
            string resultEncoded = clientNotifier.NotifyClient(encodedMessage);

            // Decode the result
            Message result = serializer.Decode(resultEncoded);

            // Run the callback before processing the result
            result.Callback(handler);

            return result;
        }

        [DataContract]
        public class Message
        {
            public virtual void Callback(THandler handler) { }
        }
    }
}
