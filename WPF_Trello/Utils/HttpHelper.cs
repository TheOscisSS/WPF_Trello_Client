using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        public static List<User> ParseJsonToUsersCredentials(string data)
        {
            var users = new List<User>();
            var joUsers = JArray.Parse(data);

            for(int i = 0; i < joUsers.Count; i++)
            {
                users.Add(ParseJsonToUserCredentials(joUsers[i].ToString()));
            }

            return users;
        }
        //public static Board ParseJsonToBoardFull(string data)
        //{
        //    var joBoard = JObject.Parse(data);

        //    var joOwner = (JObject)joBoard.SelectToken("owner");
        //    var joBoardLists = (JArray)joBoard.SelectToken("lists");
        //    var joMembers = (JArray)joBoard.SelectToken("members");

        //    var joId = (JValue)joBoard.SelectToken("_id");
        //    var joTitle = (JValue)joBoard.SelectToken("title");
        //    var joDescription = (JValue)joBoard.SelectToken("description");
        //    var joCreatedAt = (JValue)joBoard.SelectToken("createdAt");
        //    var joUpdatedAt = (JValue)joBoard.SelectToken("updatedAt");
        //}


        public static Board ParseJsonToBoard(string data)
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

        public static List<Board> ParseJsonToBoards(string data)
        {
            var boards = new List<Board>();
            var joObject = JObject.Parse(data);
            var joBoards = (JArray)joObject.SelectToken("boards");

            for (int i = 0; i < joBoards.Count; i++)
            {
                boards.Add(ParseJsonToBoard(joBoards[i].ToString()));
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
