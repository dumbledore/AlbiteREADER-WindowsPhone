using System.Runtime.Serialization;
namespace SvetlinAnkov.Albite.Core.Serialization
{
    public abstract class AlbiteMessenger<THandler>
    {
        private AlbiteSerializer<AlbiteMessage> serializer;
        private THandler handler;
        private IClientNotifier clientNotifier;

        public AlbiteMessenger(
            THandler handler,
            IClientNotifier clientNotifier,
            AlbiteSerializer<AlbiteMessage> serializer)
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
            AlbiteMessage message = serializer.Decode(serializedMessage);

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
        protected AlbiteMessage NotifyClient(AlbiteMessage message)
        {
            // Encode the message
            string encodedMessage = serializer.Encode(message);

            // Send to the client and get the encoded result
            string resultEncoded = clientNotifier.NotifyClient(encodedMessage);

            // Decode the result
            AlbiteMessage result = serializer.Decode(resultEncoded);

            // Run the callback before processing the result
            result.Callback(handler);

            return result;
        }

        [DataContract]
        public class AlbiteMessage
        {
            public virtual void Callback(THandler handler) { }
        }
    }
}
