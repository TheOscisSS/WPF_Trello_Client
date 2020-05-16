using System;
using System.Collections.Generic;
using System.Diagnostics;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using WPF_Trello.Models;
using WPF_Trello.ViewModels;
using WPF_Trello.Messages;
using WPF_Trello.Utils;
using WPF_Trello.Events;

namespace WPF_Trello.Services
{
    public class WebSocketService
    {
        private readonly MessageBusService _messageBusService;
        private readonly EventBusService _eventBusService;
        private readonly Socket _socket;
        private User _currentUser;

        public WebSocketService(MessageBusService messageBusService, EventBusService eventBusService)
        {
            _messageBusService = messageBusService;
            _eventBusService = eventBusService;

            _socket = IO.Socket("http://localhost:3000");
            _socket.On("BOARD:JOINED", (message) =>
            {
                // TODO: Add error handler;
                Debug.WriteLine(message);
            });
            _socket.On("BOARD:LEAVE", (message) =>
            {
                // TODO: Add error handler;
                Debug.WriteLine(message);
            });
            _socket.On("BOARD:NEW_ACTIVITY", async (avtivity) =>
            {
                BoardActivity AddedActivity = HttpHelper.ParseJsonToBoardActivity(avtivity.ToString());

                App.Current.Dispatcher.Invoke((Action)delegate 
                {
                    _messageBusService.SendTo<BoardViewModel>(new AddNewActivityMessage(AddedActivity));
                });
            });
            _socket.On("BOARD:NEW_MEBER", async (memberInfo) =>
            {
                var joMemberInfo = JObject.Parse(memberInfo.ToString());

                var joMember = (JObject)joMemberInfo.SelectToken("member");
                var jvSenderID = (JValue)joMemberInfo.SelectToken("senderID");

                User InvitedMember = HttpHelper.ParseJsonToUserCredentials(joMember.ToString());
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _messageBusService.SendTo<BoardViewModel>(new AddNewMemberMessage(InvitedMember, jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:NEW_LIST", async (listInfo) =>
            {
                var joListInfo = JObject.Parse(listInfo.ToString());

                var joList = (JObject)joListInfo.SelectToken("list");
                var jvSenderID = (JValue)joListInfo.SelectToken("senderID");

                BoardList AddedList = HttpHelper.ParseJsonToBoardList(joList.ToString());
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _messageBusService.SendTo<BoardViewModel>(new AddNewListMessage(AddedList, jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:NEW_CARD", async (cardInfo) =>
            {
                var joCardInfo = JObject.Parse(cardInfo.ToString());

                var joCard = (JObject)joCardInfo.SelectToken("card");
                var jvListID = (JValue)joCardInfo.SelectToken("listID");
                var jvSenderID = (JValue)joCardInfo.SelectToken("senderID");

                BoardCard AddedCard = HttpHelper.ParseJsonToBoardCard(joCard.ToString());
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _messageBusService.SendTo<BoardViewModel>(new AddNewCardMessage(AddedCard, jvListID.ToString(), jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:DELETE_MEMBER", async (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var jvBoardID = (JValue)joBoardInfo.SelectToken("boardID");
                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");
                var jvMemberID = (JValue)joBoardInfo.SelectToken("memberID");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    _messageBusService.SendTo<BoardViewModel>(new BoardDeleteMemberMessage(jvBoardID.ToString(), jvSenderID.ToString(), jvMemberID.ToString()));
                });
            });
            _socket.On("USER:NOTIFICATION", async (notify) => 
            {
                UserNotification userNotification = HttpHelper.ParseJsonToUserNotification(notify.ToString());
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _messageBusService.SendTo<MainViewModel>(new AddNewNotificationMessage(userNotification));
                });
            });
            _socket.On("USER:INVITED_BOARD", async (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var joBoard = (JObject)joBoardInfo.SelectToken("board");
                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");

                Board InvitedBoard = HttpHelper.ParseJsonToPreviewBoard(joBoard.ToString());
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _messageBusService.SendTo<HomeViewModel>(new AddNewBoardMessage(InvitedBoard, jvSenderID.ToString()));
                });
            });
            _socket.On("USER:USER:SET_ICON", async (iconInfo) =>
            {
                var joIconInfo = JObject.Parse(iconInfo.ToString());

                var jvIcon = (JValue)joIconInfo.SelectToken("icon");
                var jvSenderID = (JValue)joIconInfo.SelectToken("senderID");

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _messageBusService.SendTo<BoardViewModel>(new AddNewUserIconMessage(jvIcon.ToString(), jvSenderID.ToString()));
                });
            });
            _socket.On("USER:KICK_OUT", async (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var jvBoardID = (JValue)joBoardInfo.SelectToken("boardID");
                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");
                var jvMemberID = (JValue)joBoardInfo.SelectToken("memberID");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new KickOutMemberMessage(jvBoardID.ToString(), jvSenderID.ToString(), jvMemberID.ToString()));
                    await _messageBusService.SendTo<HomeViewModel>(new KickOutMemberMessage(jvBoardID.ToString(), jvSenderID.ToString()));
                });
            });
        }
        public void JoinIntoAccount(User user)
        {
            _currentUser = user;
            _socket.Emit("USER:JOIN", JObject.FromObject(_currentUser));
        }
        public void JoinIntoBoard(string boardID)
        {
            string requestDataIntoString = "{\"userID\":\"" + _currentUser.ID + "\",\"boardID\":\"" + boardID + "\"}";
            _socket.Emit("BOARD:JOIN", JObject.Parse(requestDataIntoString));
        }
        public void LeaveFromBoard()
        {
            string requestDataIntoString = "{\"userID\":\"" + _currentUser.ID + "\"}";
            _socket.Emit("BOARD:LEAVE", JObject.Parse(requestDataIntoString));
        }
        public void NotifyInvitedMember(string memberID)
        {
            string requestDataIntoString = "{\"userID\":\"" + _currentUser.ID + "\",\"memberID\":\"" + memberID + "\"}";
            _socket.Emit("USER:INVITED", requestDataIntoString);
        }
        public void LeaveFromAccount()
        {
            _socket.Disconnect();
        }
    }
}
