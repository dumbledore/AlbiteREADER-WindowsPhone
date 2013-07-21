using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using SvetlinAnkov.Albite.Core.Utils.Messaging;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public class AlbiteMessenger
    {
        private static readonly Type[] expectedTypes = new Type[] {
            // Host Messages
            typeof(GetPageMessage),
            typeof(GetPageResultMessage),
            typeof(GoToPageMessage),

            // Client Messages
            typeof(ClientMessage),
            typeof(ClientLoadedMessage),
        };

        private readonly JsonMessenger messenger
            = new JsonMessenger(expectedTypes);

        private readonly IClientMessenger clientMessenger;
        private readonly IHostMessenger hostMessenger;

        public AlbiteMessenger(
            IClientMessenger clientMessenger,
            IHostMessenger hostMessenger)
        {
            this.clientMessenger = clientMessenger;
            this.hostMessenger = hostMessenger;
        }

        private JsonMessenger.JsonMessage requestFromClient(
            JsonMessenger.JsonMessage requestMessage)
        {
            string requestEncoded = messenger.Encode(requestMessage);
            string resultEncoded = clientMessenger.NotifyClient(requestEncoded);
            return messenger.Decode(resultEncoded);
        }

        private void sendToClient(JsonMessenger.JsonMessage requestMessage)
        {
            string requestEncoded = messenger.Encode(requestMessage);
            clientMessenger.NotifyClient(requestEncoded);
        }

        // Public API for notifying the client
        public int Page
        {
            get
            {
                GetPageMessage requestMessage = new GetPageMessage();
                GetPageResultMessage resultMessage
                    = (GetPageResultMessage)requestFromClient(requestMessage);
                return resultMessage.Page;
            }
            set
            {
                GoToPageMessage requestMessage = new GoToPageMessage(value);
                sendToClient(requestMessage);
            }
        }

        public int PageCount
        {
            get { return 0; }
        }

        public string DomLocation
        {
            get { return null; }
            set { }
        }

        public string ElementById
        {
            set { }
        }

        // TODO: Bookmark

        // Host Messages
        [DataContract(Name = "getPage")]
        public class GetPageMessage : JsonMessenger.JsonMessage { }

        [DataContract(Name = "result_getPage")]
        public class GetPageResultMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }
        }

        [DataContract(Name = "goToPage")]
        public class GoToPageMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }

            public GoToPageMessage(int page)
            {
                Page = page;
            }
        }

        public void NotifyHost(string encodedMessage)
        {
            ClientMessage message
                = (ClientMessage) messenger.Decode(encodedMessage);
            message.Messenger = this;
            message.Callback();
        }

        // Client Messages
        [DataContract]
        public class ClientMessage : JsonMessenger.JsonMessage
        {
            public AlbiteMessenger Messenger { get; set; }
        }

        [DataContract(Name = "client_loaded")]
        public class ClientLoadedMessage : ClientMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }

            [DataMember(Name = "pageCount")]
            public int PageCount { get; private set; }

            public override void Callback()
            {
                Messenger.hostMessenger.ClientLoaded(Page, PageCount);
            }
        }

        public interface IClientMessenger
        {
            string NotifyClient(string message);
        }

        public interface IHostMessenger
        {
            // Notifications from client to host
            void ClientLoaded(int page, int pageCount);
        }
    }
}
