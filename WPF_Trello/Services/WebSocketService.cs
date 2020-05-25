using System.Diagnostics;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using WPF_Trello.Models;
using WPF_Trello.ViewModels;
using WPF_Trello.Messages;
using WPF_Trello.Utils;
using System.Threading.Tasks;

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

            _socket = IO.Socket("https://intense-temple-88335.herokuapp.com/");
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
            _socket.On("BOARD:NEW_ACTIVITY", (avtivity) =>
            {
                BoardActivity AddedActivity = HttpHelper.ParseJsonToBoardActivity(avtivity.ToString());

                App.Current.Dispatcher.Invoke(async () =>
                {
                   await _messageBusService.SendTo<BoardViewModel>(new AddNewActivityMessage(AddedActivity));
                });
            });
            _socket.On("BOARD:NEW_MEBER", (memberInfo) =>
            {
                var joMemberInfo = JObject.Parse(memberInfo.ToString());

                var joMember = (JObject)joMemberInfo.SelectToken("member");
                var jvSenderID = (JValue)joMemberInfo.SelectToken("senderID");

                User InvitedMember = HttpHelper.ParseJsonToUserCredentials(joMember.ToString());
                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new AddNewMemberMessage(InvitedMember, jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:NEW_LIST", (listInfo) =>
            {
                var joListInfo = JObject.Parse(listInfo.ToString());

                var joList = (JObject)joListInfo.SelectToken("list");
                var jvSenderID = (JValue)joListInfo.SelectToken("senderID");

                BoardList AddedList = HttpHelper.ParseJsonToBoardList(joList.ToString());
                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new AddNewListMessage(AddedList, jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:NEW_CARD", (cardInfo) =>
            {
                var joCardInfo = JObject.Parse(cardInfo.ToString());

                var joCard = (JObject)joCardInfo.SelectToken("card");
                var jvListID = (JValue)joCardInfo.SelectToken("listID");
                var jvSenderID = (JValue)joCardInfo.SelectToken("senderID");

                BoardCard AddedCard = HttpHelper.ParseJsonToBoardCard(joCard.ToString());
                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new AddNewCardMessage(AddedCard, jvListID.ToString(), jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:DELETE_MEMBER", (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var jvBoardID = (JValue)joBoardInfo.SelectToken("boardID");
                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");
                var jvMemberID = (JValue)joBoardInfo.SelectToken("memberID");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new BoardDeleteMemberMessage(jvBoardID.ToString(), jvSenderID.ToString(), jvMemberID.ToString()));
                });
            });
            _socket.On("USER:NOTIFICATION", (notify) => 
            {
                UserNotification userNotification = HttpHelper.ParseJsonToUserNotification(notify.ToString());
                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<MainViewModel>(new AddNewNotificationMessage(userNotification));
                });
            });
            _socket.On("USER:INVITED_BOARD", (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var joBoard = (JObject)joBoardInfo.SelectToken("board");
                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");

                Board InvitedBoard = HttpHelper.ParseJsonToPreviewBoard(joBoard.ToString());
                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<HomeViewModel>(new AddNewBoardMessage(InvitedBoard, jvSenderID.ToString()));
                });
            });
            _socket.On("USER:SET_ICON", (iconInfo) =>
            {
                var joIconInfo = JObject.Parse(iconInfo.ToString());

                var jvIcon = (JValue)joIconInfo.SelectToken("icon");
                var jvSenderID = (JValue)joIconInfo.SelectToken("senderID");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new AddNewUserIconMessage(jvIcon.ToString(), jvSenderID.ToString()));
                });
            });
            _socket.On("USER:KICK_OUT", (boardInfo) =>
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
            _socket.On("BOARD:MOVE_LIST", (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");
                var jvFromID = (JValue)joBoardInfo.SelectToken("from");
                var jvToID = (JValue)joBoardInfo.SelectToken("to");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new MoveBoardListMessage((int)jvFromID, (int)jvToID, jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:MOVE_CARD", (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");
                var jvListID = (JValue)joBoardInfo.SelectToken("listID");
                var jvFromID = (JValue)joBoardInfo.SelectToken("from");
                var jvToID = (JValue)joBoardInfo.SelectToken("to");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(
                        new MoveBoardCardMessage((int)jvFromID, (int)jvToID, jvSenderID.ToString(), jvListID.ToString())
                        );
                });
            });
            _socket.On("BOARD:MOVE_CARD_BETWEEN_LISTS", (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");
                var jvFromListID = (JValue)joBoardInfo.SelectToken("fromListID");
                var jvToListID = (JValue)joBoardInfo.SelectToken("toListID");
                var jvFromID = (JValue)joBoardInfo.SelectToken("from");
                var jvToID = (JValue)joBoardInfo.SelectToken("to");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(
                        new MoveCardBetweenListsMessage(jvFromListID.ToString(), jvToListID.ToString(), (int)jvFromID, (int)jvToID, jvSenderID.ToString())
                        ); 
                });
            });
            _socket.On("CARD:SET_DESCRIPTION", (cardInfo) =>
            {
                var joCardInfo = JObject.Parse(cardInfo.ToString());

                var jvSenderID = (JValue)joCardInfo.SelectToken("senderID");
                var jvCardID = (JValue)joCardInfo.SelectToken("cardID");
                var jvDescription = (JValue)joCardInfo.SelectToken("description");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(
                        new UpdateDescriptionMessage(jvSenderID.ToString(), jvCardID.ToString(), jvDescription.ToString())
                        );
                });
            });
            _socket.On("LIST:DELETE", (listInfo) =>
            {
                var joListInfo = JObject.Parse(listInfo.ToString());

                var jvSenderID = (JValue)joListInfo.SelectToken("senderID");
                var jvListID = (JValue)joListInfo.SelectToken("listID");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new DeleteBoardListMessage(jvListID.ToString(), jvSenderID.ToString()));
                });
            });
            _socket.On("CARD:DELETE", (cardInfo) =>
            {
                var joCardInfo = JObject.Parse(cardInfo.ToString());

                var jvSenderID = (JValue)joCardInfo.SelectToken("senderID");
                var jvListID = (JValue)joCardInfo.SelectToken("listID");
                var jvCardID = (JValue)joCardInfo.SelectToken("cardID");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new DeleteBoardCardMessage(jvListID.ToString(), jvCardID.ToString(), jvSenderID.ToString()));
                });
            });
            _socket.On("BOARD:DELETE", (boardInfo) =>
            {
                var joBoardInfo = JObject.Parse(boardInfo.ToString());

                var jvSenderID = (JValue)joBoardInfo.SelectToken("senderID");
                var jvBoardID = (JValue)joBoardInfo.SelectToken("boardID");

                App.Current.Dispatcher.Invoke(async () =>
                {
                    await _messageBusService.SendTo<BoardViewModel>(new DeleteBoardMessage(jvBoardID.ToString(), jvSenderID.ToString()));
                    await _messageBusService.SendTo<HomeViewModel>(new DeleteBoardMessage(jvBoardID.ToString(), jvSenderID.ToString()));
                });
            });
        }
        public void JoinIntoAccount(User user)
        {
            _currentUser = user;
            string requestDataIntoString = "{\"userID\":\"" + _currentUser.ID + "\",\"Username\":\"" + _currentUser.Username + "\"}";
            _socket.Emit("USER:JOIN", JObject.Parse(requestDataIntoString));
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
        public void MoveBoardList(int fromIndex, int toIndex, string boardID)
        {
            string requestDataIntoString = "{\"userID\":\"" + _currentUser.ID + "\", \"boardID\":\"" + boardID + "\"," +
                "\"fromIndex\":\"" + fromIndex + "\", \"toIndex\":\"" + toIndex + "\"}";
            _socket.Emit("BOARD:MOVE_LIST", JObject.Parse(requestDataIntoString));
        }
        public void LeaveFromAccount()
        {
            _socket.Emit("USER:LEAVE");
        }
    }
}
