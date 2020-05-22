using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPF_Trello.Models;
using WPF_Trello.Utils;

namespace WPF_Trello.Services
{
    public class BoardService
    {
        public async Task<List<Board>> GetAllBoards()
        {
            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Get, "board");

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToPreviewBoards(joResponse.ToString());
            }
        }
        public async Task<ObservableCollection<UserNotification>> GetAllUserNotifications()
        {
            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Get, "notification");

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToUserNotifications(joResponse.ToString());
            }
        }
        public async Task<UserNotification> ReadNotificationById(string notificationID)
        {
            StringContent strContent
                = new StringContent("{\"notificationID\":\"" + notificationID + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Put, "notification/read", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToUserNotification(joResponse.ToString());
            }
        }
        public async Task<User> KickOutMemberFromBoardById(string boardID, string memberID)
        {
            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Delete, $"board/member/{boardID}&{memberID}");

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToUserCredentials(joResponse.ToString());
            }
        }
        public async Task<Board> GetBoardById(string id)
        {
            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Get, $"board/{id}");

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                var joBoard = (JObject)joResponse.SelectToken("board");

                return HttpHelper.ParseJsonToFullBoard(joBoard.ToString());
            }
        }

        public async Task<BoardList> CreateNewList(string boardID, string title)
        {
            StringContent strContent
                = new StringContent("{\"boardID\":\"" + boardID + "\",\"title\":\"" + title + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Post, $"list/create", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToBoardList(joResponse.ToString());
            }
        }

        public async Task<BoardCard> CreateNewCard(string listID, string title)
        {
            StringContent strContent
                = new StringContent("{\"listID\":\"" + listID + "\",\"title\":\"" + title + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Post, $"card/create", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToBoardCard(joResponse.ToString());
            }
        }

        public async Task<User> InviteNewMember(string boardID, string memberName)
        {
            StringContent strContent
                = new StringContent("{\"boardID\":\"" + boardID + "\",\"memberName\":\"" + memberName + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Post, $"board/invite", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToUserCredentials(joResponse.ToString());
            }
        }
        public async Task<Board> CreateNewBoard(string title, string background)
        {
            StringContent strContent
                = new StringContent("{\"title\":\"" + title + "\",\"background\":\"" + background + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Post, $"board/create", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToPreviewBoard(joResponse.ToString());
            }
        }
        public async Task<BoardCard> SetDescription(string cardID, string description)
        {
            var replaced = Regex.Replace(description, @"\r\n|\n\n|\r\r|\n|\r", "\\r\\n");
                replaced = Regex.Replace(replaced, @"\t", "\\t");
            StringContent strContent
                = new StringContent("{\"cardID\":\"" + cardID + "\",\"description\":\"" + replaced + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Put, $"card/description", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToBoardCard(joResponse.ToString());
            }
        }
        public async Task<string> GetDescription(string cardID)
        {
            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Get, $"card/description/{cardID}");

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                var joCardDescription = JObject.Parse(joResponse.ToString());
                var jvdescription = (JValue)joCardDescription.SelectToken("description");

                return jvdescription.ToString();
            }
        }
        public async Task<List<string>> MoveBoardList(int from, int to, string boardID)
        {
            StringContent strContent
                = new StringContent("{\"boardID\":\"" + boardID + "\",\"from\":\"" + from + "\", \"to\":\"" + to + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Put, $"list/move", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                var newLists = new List<string>();

                var jaLists = JArray.Parse(joResponse.ToString());

                for (int i = 0; i < jaLists.Count; i++)
                {
                    newLists.Add(jaLists[i].ToString());
                }

                return newLists;
            }
        }
        public async Task<List<string>> MoveBoardCard(int from, int to, string listID)
        {
            StringContent strContent
                = new StringContent("{\"listID\":\"" + listID + "\",\"from\":\"" + from + "\", \"to\":\"" + to + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Put, $"card/move", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                var newCards= new List<string>();

                var jaCards = JArray.Parse(joResponse.ToString());

                for (int i = 0; i < jaCards.Count; i++)
                {
                    newCards.Add(jaCards[i].ToString());
                }

                return newCards;
            }
        }
        public async Task<bool> MoveBoardCardBetweenLists(int from, int to, string fromListID, string toListID)
        {
            StringContent strContent
                = new StringContent("{\"fromListID\":\"" + fromListID + "\", \"toListID\":\"" + toListID + "\", \"from\":\"" + from + "\", \"to\":\"" + to + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Put, $"card/moveToAnotherList", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return true;
            }
        }
        public async Task<ObservableCollection<PictureExample>> GetRandomPicture(string query = "wallpaper", string orientation = "landscape", int count = 9)
        {
            HttpResponseMessage response = await HttpHelper.HttpRequestForUnsplash(HttpMethod.Get, $"photos/random?query={query}&count={count}&orientation={orientation}");

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    //TODO: Generate suitable exception 
                    throw new Exception("May be time is over");
                }

                return HttpHelper.ParseJsonToPicturesUrl(result);
            }
        }
    }
}
