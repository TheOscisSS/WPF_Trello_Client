using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Utils
{
    public class HttpHelper
    {
        public static User ParseJsonToUserCredentials(string data)
        {
                var joUser = JObject.Parse(data);

                //var joUser = (JObject)joResponse.SelectToken("user");
                var joId = (JValue)joUser.SelectToken("_id");
                var joUsername = (JValue)joUser.SelectToken("username");
                var joCreatedAt = (JValue)joUser.SelectToken("createdAt");
                var joUpdatedAt = (JValue)joUser.SelectToken("updatedAt");


                return new User(
                    joId.ToString(),
                    joUsername.ToString(),
                    DateTime.Parse(joCreatedAt.ToString()),
                    DateTime.Parse(joUpdatedAt.ToString()),
                    new string[] { });
        }   
    }
}
