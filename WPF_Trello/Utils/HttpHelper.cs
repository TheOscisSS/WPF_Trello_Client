using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Utils
{
    public class HttpHelper
    {
        private const string URI = "http://localhost:3000/";
        public static User ParseJsonToUserCredentials(string data)
        {
            var joUser = JObject.Parse(data);

            var joId = (JValue)joUser.SelectToken("_id");
            var joUsername = (JValue)joUser.SelectToken("username");
            var joCreatedAt = (JValue)joUser.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joUser.SelectToken("updatedAt");


            return new User(
                joId.ToString(),
                joUsername.ToString(),
                DateTime.Parse(joCreatedAt.ToString()),
                DateTime.Parse(joUpdatedAt.ToString()),
                new string[] { }
                );
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
            var jaLists = (JArray)joBoard.SelectToken("lists");

            var joId = (JValue)joBoard.SelectToken("_id");
            var joTitle = (JValue)joBoard.SelectToken("title");
            var joDescription = (JValue)joBoard.SelectToken("description");
            var joBackground = (JValue)joBoard.SelectToken("background");
            var joCreatedAt = (JValue)joBoard.SelectToken("createdAt");
            var joUpdatedAt = (JValue)joBoard.SelectToken("updatedAt");

            User Owner = ParseJsonToUserCredentials(joOwner.ToString());
            ObservableCollection<User> Members = ParseJsonToUsersCredentials(jaMembers.ToString());
            ObservableCollection<BoardList> BoardLists = ParseJsonToBoardLists(jaLists.ToString());

            return new Board(
                joId.ToString(),
                joTitle.ToString(),
                joDescription.ToString(),
                joBackground.ToString(),
                Owner,
                Members,
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
