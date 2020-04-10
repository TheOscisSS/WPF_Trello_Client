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

                return HttpHelper.ParseJsonToBoards(joResponse.ToString());
            }
        }
    }
}
