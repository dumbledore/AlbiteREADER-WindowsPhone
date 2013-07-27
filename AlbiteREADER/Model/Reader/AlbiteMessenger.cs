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
    internal class AlbiteMessenger
    {
        private static readonly Type[] expectedTypes = new Type[] {
            // Error
            typeof(ErrorMessage),

            // Host Messages
            typeof(GetPageMessage),
            typeof(GetPageResultMessage),
            typeof(GoToPageMessage),
            typeof(GoToPageResultMessage),
            typeof(GetDomLocationMessage),
            typeof(GetDomLocationResultMessage),
            typeof(GoToDomLocationMessage),
            typeof(GoToDomLocationResultMessage),
            typeof(GoToElementByIdMessage),
            typeof(GoToElementByIdResultMessage),
            typeof(GetBookmarkMessage),
            typeof(GetBookmarkResultMessage),

            // Used by the Host Messages
            typeof(Bookmark),

            // Client Messages
            typeof(ClientLoadedMessage),
            typeof(GoToPrevoiusChapterMessage),
            typeof(GoToNextChapterMessage),
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
            // Encode the JsonMessage to a string
            string requestEncoded = messenger.Encode(requestMessage);

            // Send the encoded message to the client and retrieve the result
            string resultEncoded = clientMessenger.NotifyClient(requestEncoded);

            // Decode back to a JsonMessage
            JsonMessenger.JsonMessage result = messenger.Decode(resultEncoded);

            // Run the callback before processing the result
            result.Callback(this);

            return result;
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
                requestFromClient(requestMessage);
            }
        }

        public int PageCount { get; private set; }

        public int FirstPage { get { return 1; } }

        public int LastPage { get { return PageCount; } }

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
                requestFromClient(requestMessage);
            }
        }

        public void GoToElementById(string id)
        {
            GoToElementByIdMessage requestMessage = new GoToElementByIdMessage(id);
            requestFromClient(requestMessage);
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

        // Error
        public class MessengerException : Exception
        {
            public MessengerException(string message)
                : base(message) { }

            public MessengerException(
                string name,
                string message,
                string stack)
            : this(string.Format("Name: {0}\nMessage: {1}\nStack:\n{2}",
                name, message, stack)) { }
        }

        [DataContract(Name = "error")]
        private class ErrorMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "name")]
            public string Name { get; private set; }

            [DataMember(Name = "message")]
            public string Message { get; private set; }

            [DataMember(Name = "stack")]
            public string Stack { get; private set; }

            public override void Callback(object data)
            {
                throw new MessengerException(
                    Name, Message, Stack);
            }
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

        [DataContract(Name = "result_goToPage")]
        private class GoToPageResultMessage : JsonMessenger.JsonMessage { }

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

        [DataContract(Name = "result_goToDomLocation")]
        private class GoToDomLocationResultMessage : JsonMessenger.JsonMessage { }

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

        [DataContract(Name = "result_goToElementById")]
        private class GoToElementByIdResultMessage : JsonMessenger.JsonMessage { }

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
            // Receiving a notification from client
            // Decode to JsonMessage
            JsonMessenger.JsonMessage message
                = messenger.Decode(encodedMessage);

            // Running the callback will make sure
            // the message does its job
            message.Callback(this);
        }

        // Client Messages
        [DataContract(Name = "client_loaded")]
        private class ClientLoadedMessage : JsonMessenger.JsonMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }

            [DataMember(Name = "pageCount")]
            public int PageCount { get; private set; }

            public override void Callback(object data)
            {
                AlbiteMessenger messenger = (AlbiteMessenger)data;

                // Don't update Page as it will cause a GoToPageMessage
                // We don't need to update it anyway

                // Update  the PageCount as it's cached locally
                messenger.PageCount = PageCount;
                messenger.hostMessenger.ClientLoaded(Page, PageCount);
            }
        }

        [DataContract(Name="client_goToPreviousChapter")]
        private class GoToPrevoiusChapterMessage : JsonMessenger.JsonMessage
        {
            public override void Callback(object data)
            {
                AlbiteMessenger messenger = (AlbiteMessenger)data;
                messenger.hostMessenger.GoToPreviousChapter();
            }
        }

        [DataContract(Name = "client_goToNextChapter")]
        private class GoToNextChapterMessage : JsonMessenger.JsonMessage
        {
            public override void Callback(object data)
            {
                AlbiteMessenger messenger = (AlbiteMessenger)data;
                messenger.hostMessenger.GoToNextChapter();
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
            void GoToPreviousChapter();
            void GoToNextChapter();
        }
    }
}
