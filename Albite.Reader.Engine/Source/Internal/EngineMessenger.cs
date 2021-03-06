﻿using Albite.Reader.BookLibrary.Location;
using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Core.Json;
using Albite.Reader.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace Albite.Reader.Engine.Internal
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
            typeof(GetDomLocationMessage),
            typeof(GetDomLocationResultMessage),
            typeof(GoToLocationMessage),
            typeof(GoToLocationResultMessage),
            typeof(GoToElementByIdMessage),
            typeof(GoToElementByIdResultMessage),
            typeof(ShowStatusBarMessage),
            typeof(ShowStatusBarResultMessage),
            typeof(HideStatusBarMessage),
            typeof(HideStatusBarResultMessage),
            typeof(GetBookmarkMessage),
            typeof(GetBookmarkResultMessage),

            // Used by the Host Messages
            typeof(Bookmark),
            typeof(ChapterLocation),
            typeof(FirstPageLocation),
            typeof(LastPageLocation),
            typeof(PageLocation),
            typeof(ElementLocation),
            typeof(DomLocation),
            typeof(RelativeChapterLocation),

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

        private static string typeNameSpace
            = ":#" + typeof(EngineMessenger).Namespace;

        public static string TypeNamespace {
            get { return typeNameSpace; }
        }

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
                GoToLocationMessage requestMessage
                    = new GoToLocationMessage(new PageLocation(value));
                NotifyClient(requestMessage);
            }
        }

        public ChapterLocation Location
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
                GoToLocationMessage requestMessage = new GoToLocationMessage(value);
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

        public bool StatusBarShown
        {
            set
            {
                Message requestMessage;
                if (value)
                {
                    requestMessage = new ShowStatusBarMessage();
                }
                else
                {
                    requestMessage = new HideStatusBarMessage();
                }
                NotifyClient(requestMessage);
            }
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
        private class ErrorMessage : Message
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
        private class GetPageMessage : Message { }

        [DataContract(Name = "result_getPage")]
        private class GetPageResultMessage : Message
        {
            [DataMember(Name = "page")]
            public int Page { get; private set; }
        }

        [DataContract(Name = "getDomLocation")]
        private class GetDomLocationMessage : Message { }

        [DataContract(Name = "result_getDomLocation")]
        private class GetDomLocationResultMessage : Message
        {
            [DataMember(Name = "location")]
            public DomLocation DomLocation { get; private set; }
        }

        [DataContract(Name = "goToLocation")]
        private class GoToLocationMessage : Message
        {
            [DataMember(Name = "location")]
            public ChapterLocation Location { get; private set; }

            public GoToLocationMessage(ChapterLocation location)
            {
                Location = location;
            }
        }

        [DataContract(Name = "result_goToLocation")]
        private class GoToLocationResultMessage : Message { }

        [DataContract(Name = "goToElementById")]
        private class GoToElementByIdMessage : Message
        {
            [DataMember(Name = "elementId")]
            public string Id { get; private set; }

            public GoToElementByIdMessage(string id)
            {
                Id = id;
            }
        }

        [DataContract(Name = "result_goToElementById")]
        private class GoToElementByIdResultMessage : Message { }

        [DataContract(Name = "showStatusBar")]
        private class ShowStatusBarMessage : Message { }

        [DataContract(Name = "result_showStatusBar")]
        private class ShowStatusBarResultMessage : Message { }

        [DataContract(Name = "hideStatusBar")]
        private class HideStatusBarMessage : Message { }

        [DataContract(Name = "result_hideStatusBar")]
        private class HideStatusBarResultMessage : Message { }

        [DataContract(Name = "getBookmark")]
        private class GetBookmarkMessage : Message { }

        [DataContract(Name = "result_getBookmark")]
        private class GetBookmarkResultMessage : Message
        {
            [DataMember(Name = "bookmark")]
            public Bookmark Bookmark { get; private set; }
        }

        // Client Messages
        [DataContract(Name = "client_log")]
        private class ClientLogMessage : Message
        {
            [DataMember(Name = "message")]
            public string Message { get; private set; }

            public override void Callback(IClientHandler handler)
            {
                handler.ClientLog(Message);
            }
        }

        [DataContract(Name = "client_loaded")]
        private class ClientLoadedMessage : Message
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
        private class GoToPrevoiusChapterMessage : Message
        {
            public override void Callback(IClientHandler handler)
            {
                handler.GoToPreviousChapter();
            }
        }

        [DataContract(Name = "client_goToNextChapter")]
        private class GoToNextChapterMessage : Message
        {
            public override void Callback(IClientHandler handler)
            {
                handler.GoToNextChapter();
            }
        }

        [DataContract(Name = "client_navigate")]
        private class ClientNavigateMessage : Message
        {
            [DataMember(Name = "url")]
            public string Url { get; private set; }

            [DataMember(Name = "title")]
            public string Title { get; private set; }

            public override void Callback(IClientHandler handler)
            {
                handler.ClientNavigationRequest(Url, Title);
            }
        }

        [DataContract(Name = "client_toggleFullscreen")]
        private class ClientToggleFullscreenMessage : Message
        {
            public override void Callback(IClientHandler handler)
            {
                //TODO
                Log.D(tag, "toggleFullScreen()");
            }
        }

        [DataContract(Name = "client_contextMenu")]
        private class ClientContextMenuMessage : Message
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
            void ClientNavigationRequest(string url, string title);
        }
    }
}
