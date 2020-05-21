using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Utils
{
    public class HttpHelper
    {
        private const string ACCESS_KEY = "p_7Ur9RvtASiNnAupH2QqQ9LfWR1NMmV35uBi3uJfL8";
        private const string URI = "http://localhost:3000/";
        private const string URI_UNSPLASH = "https://api.unsplash.com/";
        public static User ParseJsonToUserCredentials(string data)
        {
            var joUser = JObject.Parse(data);

            var joId = (JValue)joUser.SelectToken("_id");
            var joIcon = (JValue)joUser.SelectToken("icon");
            var joUsername = (JValue)joUser.SelectToken("username");
            var joCreatedAt = (JValue)joUser.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joUser.SelectToken("updatedAt");


            return new User(
                joId.ToString(),
                joUsername.ToString(),
                joIcon.ToString(),
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString()),
                new string[] { }
                );
        }
        public static BoardActivity ParseJsonToBoardActivity(string data)
        {
            var joActivity = JObject.Parse(data);
            var joSender = (JObject)joActivity.SelectToken("sender");

            var joId = (JValue)joActivity.SelectToken("_id");
            var joMessage = (JValue)joActivity.SelectToken("message");
            var joCreatedAt = (JValue)joActivity.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joActivity.SelectToken("updatedAt");

            User Sender = ParseJsonToUserCredentials(joSender.ToString());

            return new BoardActivity(
                joId.ToString(),
                joMessage.ToString(),
                Sender,
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString())
                );
        }
        public static ObservableCollection<BoardActivity> ParseJsonToBoardActivities(string data)
        {
            var activities = new ObservableCollection<BoardActivity>();
            var jaActivities = JArray.Parse(data);

            for(int i = 0; i < jaActivities.Count; i++)
            {
                activities.Add(ParseJsonToBoardActivity(jaActivities[i].ToString()));
            }

            return activities;
        }
        public static UserNotification ParseJsonToUserNotification(string data)
        {
            var joNotification = JObject.Parse(data);

            var joId = (JValue)joNotification.SelectToken("_id");
            var joMessage = (JValue)joNotification.SelectToken("message");
            var joIsReaded = (JValue)joNotification.SelectToken("isReaded");
            var joCreatedAt = (JValue)joNotification.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joNotification.SelectToken("updatedAt");

            return new UserNotification(
                joId.ToString(),
                joMessage.ToString(),
                (bool)joIsReaded,
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString())
                );
        }
        public static ObservableCollection<UserNotification> ParseJsonToUserNotifications(string data)
        {
            var notifications = new ObservableCollection<UserNotification>();
            var joObject = JObject.Parse(data);
            var jaNotifications = (JArray)joObject.SelectToken("notifications");

            for(int i = 0; i < jaNotifications.Count; i++)
            {
                notifications.Add(ParseJsonToUserNotification(jaNotifications[i].ToString()));
            }

            return notifications;
        }
        public static ObservableCollection<User> ParseJsonToUsersCredentials(string data)
        {
            var users = new ObservableCollection<User>();
            var jaUsers = JArray.Parse(data);

            for(int i = 0; i < jaUsers.Count; i++)
            {
                users.Add(ParseJsonToUserCredentials(jaUsers[i].ToString()));
            }

            return users;
        }
        public static BoardCard ParseJsonToBoardCard(string data)
        {
            var joBoardCard = JObject.Parse(data);

            var joId = (JValue)joBoardCard.SelectToken("_id");
            var joTitle = (JValue)joBoardCard.SelectToken("title");
            var joDescription = (JValue)joBoardCard.SelectToken("description");
            var joCreatedAt = (JValue)joBoardCard.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joBoardCard.SelectToken("updatedAt");

            return new BoardCard(
                joId.ToString(), 
                joTitle.ToString(), 
                joDescription.ToString(), 
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString()));
        }
        public static ObservableCollection<BoardCard> ParseJsonToBoardCards(string data)
        {
            var boardCards = new ObservableCollection<BoardCard>();
            var jaBoardCards = JArray.Parse(data);

            for (int i = 0; i < jaBoardCards.Count; i++)
            {
                boardCards.Add(ParseJsonToBoardCard(jaBoardCards[i].ToString()));
            }

            return boardCards;
        }
        public static BoardList ParseJsonToBoardList(string data)
        {
            var joBoardList = JObject.Parse(data);

            var jaCards = (JArray)joBoardList.SelectToken("cards");

            var joId = (JValue)joBoardList.SelectToken("_id");
            var joTitle = (JValue)joBoardList.SelectToken("title");
            var joCreatedAt = (JValue)joBoardList.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joBoardList.SelectToken("updatedAt");

            ObservableCollection<BoardCard> Cards = ParseJsonToBoardCards(jaCards.ToString());

            return new BoardList(
                joId.ToString(),
                joTitle.ToString(),
                Cards,
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString())
                );
        }
        public static ObservableCollection<BoardList> ParseJsonToBoardLists(string data)
        {
            var boardLists = new ObservableCollection<BoardList>();
            var jaBoardLists = JArray.Parse(data);

            for(int i = 0; i < jaBoardLists.Count; i++)
            {
                boardLists.Add(ParseJsonToBoardList(jaBoardLists[i].ToString()));
            }

            return boardLists;
        }
        public static Board ParseJsonToFullBoard(string data)
        {
            var joBoard = JObject.Parse(data);
            var joOwner = (JObject)joBoard.SelectToken("owner");

            var jaMembers = (JArray)joBoard.SelectToken("members");
            var jaActivities = (JArray)joBoard.SelectToken("activities");
            var jaLists = (JArray)joBoard.SelectToken("lists");

            var joId = (JValue)joBoard.SelectToken("_id");
            var joTitle = (JValue)joBoard.SelectToken("title");
            var joDescription = (JValue)joBoard.SelectToken("description");
            var joBackground = (JValue)joBoard.SelectToken("background");
            var joCreatedAt = (JValue)joBoard.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joBoard.SelectToken("updatedAt");

            User Owner = ParseJsonToUserCredentials(joOwner.ToString());
            ObservableCollection<User> Members = ParseJsonToUsersCredentials(jaMembers.ToString());
            ObservableCollection<BoardActivity> Activities = ParseJsonToBoardActivities(jaActivities.ToString());
            ObservableCollection<BoardList> BoardLists = ParseJsonToBoardLists(jaLists.ToString());

            return new Board(
                joId.ToString(),
                joTitle.ToString(),
                joDescription.ToString(),
                joBackground.ToString(),
                Owner,
                Members,
                Activities,
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString()),
                BoardLists
                );
        }
        public static Board ParseJsonToPreviewBoard(string data)
        {
            var joBoard = JObject.Parse(data);


            var joId = (JValue)joBoard.SelectToken("_id");
            var joTitle = (JValue)joBoard.SelectToken("title");
            var joDescription = (JValue)joBoard.SelectToken("description");
            var joBackground = (JValue)joBoard.SelectToken("background");
            var joCreatedAt = (JValue)joBoard.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joBoard.SelectToken("updatedAt");

            return new Board(
                joId.ToString(),
                joTitle.ToString(),
                joDescription.ToString(),
                joBackground.ToString(),
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString())
                ); ;
        }
        public static List<Board> ParseJsonToPreviewBoards(string data)
        {
            var boards = new List<Board>();
            var joObject = JObject.Parse(data);
            var jaBoards = (JArray)joObject.SelectToken("boards");

            for (int i = 0; i < jaBoards.Count; i++)
            {
                boards.Add(ParseJsonToPreviewBoard(jaBoards[i].ToString()));
            }

            return boards;
        }
        public static PictureExample ParseJsonToPictureUrl(string data)
        {
            var joObject = JObject.Parse(data);
            var joUrls = (JObject)joObject.SelectToken("urls");
            
            var joRegular = (JValue)joUrls.SelectToken("regular");
            var joSmall = (JValue)joUrls.SelectToken("small");
            var joThumb = (JValue)joUrls.SelectToken("thumb");

            return new PictureExample(joRegular.ToString(), joSmall.ToString(), joThumb.ToString());
        }
        public static ObservableCollection<PictureExample> ParseJsonToPicturesUrl(string data)
        {
            var PictureCollection = new ObservableCollection<PictureExample>();
            var jaPicruteCollection = JArray.Parse(data);

            for (int i = 0; i < jaPicruteCollection.Count; i++)
            {
                PictureCollection.Add(ParseJsonToPictureUrl(jaPicruteCollection[i].ToString()));
            }

            return PictureCollection;

        }
        public async static Task<HttpResponseMessage> HttpRequestForUnsplash(HttpMethod method, string requestURI)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URI_UNSPLASH);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Client-ID", ACCESS_KEY);
                using (HttpRequestMessage request = new HttpRequestMessage(method, requestURI))
                {
                    return await client.SendAsync(request);
                }
            }
        }
        public async static Task<HttpResponseMessage> HttpRequest(HttpMethod method, string requestURI)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URI);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (AccessToken.isExist())
                {
                    client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", await AccessToken.Load());
                }
                using (HttpRequestMessage request = new HttpRequestMessage(method, requestURI))
                {
                    return await client.SendAsync(request);
                }
            }
        }
        public async static Task<HttpResponseMessage> HttpRequest(HttpMethod method, string requestURI, StringContent content)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URI);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (AccessToken.isExist())
                {
                    client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", await AccessToken.Load());
                }
                using (HttpRequestMessage request = new HttpRequestMessage(method, requestURI))
                {
                    request.Content = content;

                    return await client.SendAsync(request);
                }
            }
        }
    }
}
