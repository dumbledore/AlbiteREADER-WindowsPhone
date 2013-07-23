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
            typeof(GetDomLocationMessage),
            typeof(GetDomLocationResultMessage),
            typeof(GoToDomLocationMessage),
            typeof(GoToElementByIdMessage),
            typeof(GetBookmarkMessage),
            typeof(GetBookmarkResultMessage),

            // Used by the Host Messages
            typeof(Bookmark),

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

        public int PageCount { get; private set; }

        public string DomLocation
        {
            get
            {
                GetDomLocationMessage requestMessage = new GetDomLocationMessage();
                GetDomLocationResultMessage resultMessage
                    = (GetDomLocationResultMessage)requestFromClient(requestMessage);
                return resultMessage.Location;
            }

            set
            {
                GoToDomLocationMessage requestMessage = new GoToDomLocationMessage(value);
                sendToClient(requestMessage);
            }
        }

        public void GoToElementById(string id)
        {
            GoToElementByIdMessage requestMessage = new GoToElementByIdMessage(id);
            sendToClient(requestMessage);
        }

        [DataContract(Name="bookmark")]
        public class Bookmark
        {
            [DataMember(Name="text")]
            public string Text { get; private set; }

            [DataMember(Name="location")]
            public string Location { get; private set; }
        }

        public Bookmark GetBookmark()
        {
            GetBookmarkMessage requestMessage = new GetBookmarkMessage();
            GetBookmarkResultMessage resultMessage
                = (GetBookmarkResultMessage)requestFromClient(requestMessage);
            return resultMessage.Bookmark;
        }

        // Host Messages
        [DataContract(Name = "getPage")]
        private class GetPageMessage : JsonMessenger.JsonMessage { }

        [DataContract(Name = "result_getPage")]
        private class GetPageResultMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }
        }

        [DataContract(Name = "goToPage")]
        private class GoToPageMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }

            public GoToPageMessage(int page)
            {
                Page = page;
            }
        }

        [DataContract(Name = "getDomLocation")]
        private class GetDomLocationMessage : JsonMessenger.JsonMessage { }

        [DataContract(Name = "result_getDomLocation")]
        private class GetDomLocationResultMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "location")]
            public string Location { get; private set; }
        }

        [DataContract(Name = "goToDomLocation")]
        private class GoToDomLocationMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "location")]
            public string Location { get; private set; }

            public GoToDomLocationMessage(string location)
            {
                Location = location;
            }
        }

        [DataContract(Name = "goToElementById")]
        private class GoToElementByIdMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "id")]
            public string Id { get; private set; }

            public GoToElementByIdMessage(string id)
            {
                Id = id;
            }
        }

        [DataContract(Name = "getBookmark")]
        private class GetBookmarkMessage : JsonMessenger.JsonMessage { }

        [DataContract(Name = "result_getBookmark")]
        private class GetBookmarkResultMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "bookmark")]
            public Bookmark Bookmark { get; private set; }
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
        private class ClientMessage : JsonMessenger.JsonMessage
        {
            public AlbiteMessenger Messenger { get; set; }
        }

        [DataContract(Name = "client_loaded")]
        private class ClientLoadedMessage : ClientMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }

            [DataMember(Name = "pageCount")]
            public int PageCount { get; private set; }

            public override void Callback()
            {
                Messenger.PageCount = PageCount;
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
