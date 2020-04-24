using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
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
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToPreviewBoards(joResponse.ToString());
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
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                return HttpHelper.ParseJsonToBoardCard(joResponse.ToString());
            }
        }
    }
}
