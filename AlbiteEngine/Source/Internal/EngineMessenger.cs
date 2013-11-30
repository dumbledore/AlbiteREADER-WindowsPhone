using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.Json;
using SvetlinAnkov.Albite.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.Engine.Internal
{
    internal class EngineMessenger : JsonMessenger<EngineMessenger.IClientHandler>
    {
        private static readonly string tag = "EngineMessenger";

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
            typeof(DomLocation),

            // Client Messages
            typeof(ClientLogMessage),
            typeof(ClientLoadedMessage),
            typeof(GoToPrevoiusChapterMessage),
            typeof(GoToNextChapterMessage),
            typeof(ClientToggleFullscreenMessage),
            typeof(ClientNavigateMessage),
            typeof(ClientContextMenuMessage),

            // Used by the Client Messages
            typeof(ContextMenuOptions),
        };

        public EngineMessenger(
            IClientHandler handler,
            IClientNotifier notifier)
            : base(handler, notifier, expectedTypes) { }

        // Public API for notifying the client
        public int Page
        {
            get
            {
                GetPageMessage requestMessage = new GetPageMessage();
                GetPageResultMessage resultMessage
                    = (GetPageResultMessage)NotifyClient(requestMessage);
                return resultMessage.Page;
            }

            set
            {
                GoToPageMessage requestMessage = new GoToPageMessage(value);
                NotifyClient(requestMessage);
            }
        }

        public DomLocation DomLocation
        {
            get
            {
                GetDomLocationMessage requestMessage = new GetDomLocationMessage();
                GetDomLocationResultMessage resultMessage
                    = (GetDomLocationResultMessage)NotifyClient(requestMessage);
                return resultMessage.DomLocation;
            }

            set
            {
                GoToDomLocationMessage requestMessage = new GoToDomLocationMessage(value);
                NotifyClient(requestMessage);
            }
        }

        public void GoToElementById(string id)
        {
            GoToElementByIdMessage requestMessage = new GoToElementByIdMessage(id);
            NotifyClient(requestMessage);
        }

        [DataContract(Name="bookmark")]
        public class Bookmark
        {
            [DataMember(Name="text")]
            public string Text { get; private set; }

            [DataMember(Name="location")]
            public DomLocation DomLocation { get; private set; }
        }

        public Bookmark GetBookmark()
        {
            GetBookmarkMessage requestMessage = new GetBookmarkMessage();
            GetBookmarkResultMessage resultMessage
                = (GetBookmarkResultMessage)NotifyClient(requestMessage);
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
        private class ErrorMessage : AlbiteMessage
        {
            [DataMember(Name = "name")]
            public string Name { get; private set; }

            [DataMember(Name = "message")]
            public string Message { get; private set; }

            [DataMember(Name = "stack")]
            public string Stack { get; private set; }

            public override void Callback(IClientHandler handler)
            {
                throw new MessengerException(
                    Name, Message, Stack);
            }
        }

        // Host Messages
        [DataContract(Name = "getPage")]
        private class GetPageMessage : AlbiteMessage { }

        [DataContract(Name = "result_getPage")]
        private class GetPageResultMessage : AlbiteMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }
        }

        [DataContract(Name = "goToPage")]
        private class GoToPageMessage : AlbiteMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }

            public GoToPageMessage(int page)
            {
                Page = page;
            }
        }

        [DataContract(Name = "result_goToPage")]
        private class GoToPageResultMessage : AlbiteMessage { }

        [DataContract(Name = "getDomLocation")]
        private class GetDomLocationMessage : AlbiteMessage { }

        [DataContract(Name = "result_getDomLocation")]
        private class GetDomLocationResultMessage : AlbiteMessage
        {
            [DataMember(Name = "location")]
            public DomLocation DomLocation { get; private set; }
        }

        [DataContract(Name = "goToDomLocation")]
        private class GoToDomLocationMessage : AlbiteMessage
        {
            [DataMember(Name = "location")]
            public DomLocation DomLocation { get; private set; }

            public GoToDomLocationMessage(DomLocation domLocation)
            {
                DomLocation = domLocation;
            }
        }

        [DataContract(Name = "result_goToDomLocation")]
        private class GoToDomLocationResultMessage : AlbiteMessage { }

        [DataContract(Name = "goToElementById")]
        private class GoToElementByIdMessage : AlbiteMessage
        {
            [DataMember(Name = "elementId")]
            public string Id { get; private set; }

            public GoToElementByIdMessage(string id)
            {
                Id = id;
            }
        }

        [DataContract(Name = "result_goToElementById")]
        private class GoToElementByIdResultMessage : AlbiteMessage { }

        [DataContract(Name = "getBookmark")]
        private class GetBookmarkMessage : AlbiteMessage { }

        [DataContract(Name = "result_getBookmark")]
        private class GetBookmarkResultMessage : AlbiteMessage
        {
            [DataMember(Name = "bookmark")]
            public Bookmark Bookmark { get; private set; }
        }

        // Client Messages
        [DataContract(Name = "client_log")]
        private class ClientLogMessage : AlbiteMessage
        {
            [DataMember(Name = "message")]
            public string Message { get; private set; }

            public override void Callback(IClientHandler handler)
            {
                handler.ClientLog(Message);
            }
        }

        [DataContract(Name = "client_loaded")]
        private class ClientLoadedMessage : AlbiteMessage
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }

            [DataMember(Name = "pageCount")]
            public int PageCount { get; private set; }

            public override void Callback(IClientHandler handler)
            {
                // Don't update Page as it will cause a GoToPageMessage
                // We don't need to update it anyway
                handler.ClientLoaded(Page, PageCount);
            }
        }

        [DataContract(Name="client_goToPreviousChapter")]
        private class GoToPrevoiusChapterMessage : AlbiteMessage
        {
            public override void Callback(IClientHandler handler)
            {
                handler.GoToPreviousChapter();
            }
        }

        [DataContract(Name = "client_goToNextChapter")]
        private class GoToNextChapterMessage : AlbiteMessage
        {
            public override void Callback(IClientHandler handler)
            {
                handler.GoToNextChapter();
            }
        }

        [DataContract(Name = "client_navigate")]
        private class ClientNavigateMessage : AlbiteMessage
        {
            [DataMember(Name = "url")]
            public string Url { get; private set; }

            public override void Callback(IClientHandler handler)
            {
                //TODO
                handler.ClientNavigationRequest(Url);
            }
        }

        [DataContract(Name = "client_toggleFullscreen")]
        private class ClientToggleFullscreenMessage : AlbiteMessage
        {
            public override void Callback(IClientHandler handler)
            {
                //TODO
                Log.D(tag, "toggleFullScreen()");
            }
        }

        [DataContract(Name = "client_contextMenu")]
        private class ClientContextMenuMessage : AlbiteMessage
        {
            [DataMember(Name = "options")]
            public ContextMenuOptions Options { get; private set; }

            public override void Callback(IClientHandler handler)
            {
                // TODO:
                Log.D(tag, string.Format(
                    "ContextMenu(options: ({0}, {1}) image={2} anchor={3})",
                    Options.PositionX, Options.PositionY,
                    Options.Image,
                    Options.Anchor));
            }
        }

        [DataContract(Name = "contextMenuOptions")]
        public class ContextMenuOptions
        {
            [DataMember(Name = "position_x")]
            public int PositionX { get; private set; }

            [DataMember(Name = "position_y")]
            public int PositionY { get; private set; }

            [DataMember(Name = "image")]
            public string Image { get; private set; }

            [DataMember(Name = "anchor")]
            public string Anchor { get; private set; }
        }

        public interface IClientHandler
        {
            // Notifications from client to host
            void ClientLog(string message);
            void ClientLoaded(int page, int pageCount);
            void GoToPreviousChapter();
            void GoToNextChapter();
            void ClientNavigationRequest(string url);
        }
    }
}
