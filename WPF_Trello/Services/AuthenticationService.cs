using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_Trello.Models;
using WPF_Trello.Utils;

namespace WPF_Trello.Services
{
    public class AuthenticationService
    {
        public User CurrentUser { get; private set; }
        public async Task<User> GetCurrentUser()
        {
            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Get, "user/me");

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                CurrentUser = HttpHelper.ParseJsonToUserCredentials(joResponse.ToString());
                return CurrentUser;
            }
        }
        public void SetCustomPrincipal(User user)
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;

            if (customPrincipal == null)
            {
                throw new ArgumentException("Thread principal must be set to CustomerPrincipal");
            }

            customPrincipal.Identity = new CustomIdentity(user.Username, new string[] { });

            //ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        }
        public async Task<User> AuthenticateUser(string username, string password)
        {
            StringContent strContent 
                = new StringContent("{\"username\":\""+ username +"\",\"password\":\""+ password +"\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Post, "user/signin", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                var joToken = (JValue)joResponse.SelectToken("token");
                var joUser = (JObject)joResponse.SelectToken("user");

                AccessToken.Save(joToken.ToString());
                
                CurrentUser = HttpHelper.ParseJsonToUserCredentials(joUser.ToString());
                return CurrentUser;
            }
        }

        public async Task<User> SetUserIcon(string icon)
        {
            StringContent strContent
                = new StringContent("{\"icon\":\"" + icon + "\"}", Encoding.UTF8,"application/json");
            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Post, "user/icon", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                CurrentUser.SetIcon(icon);
                return CurrentUser;
            }
        }

        public async Task<User> CreateUser(string username, string password, string confirmPassword)
        {
            StringContent strContent
                = new StringContent("{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"confirmPassword\":\"" + confirmPassword + "\"}",
                                    Encoding.UTF8,
                                    "application/json");

            HttpResponseMessage response = await HttpHelper.HttpRequest(HttpMethod.Post, "user/signup", strContent);

            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var joResponse = JObject.Parse(result);

                if (!response.IsSuccessStatusCode)
                {
                    var joMessage = (JValue)joResponse.SelectToken("message");
                    throw new UnauthorizedAccessException(joMessage.ToString());
                }

                var joToken = (JValue)joResponse.SelectToken("token");
                var joUser = (JObject)joResponse.SelectToken("user");

                AccessToken.Save(joToken.ToString());


                CurrentUser = HttpHelper.ParseJsonToUserCredentials(joUser.ToString());
                return CurrentUser;
            }
        }

        public event Action<bool> OnStatusChanged;

        public void ChangeStatus(bool status) => OnStatusChanged?.Invoke(status);
    }

    public class HttpResponseException : Exception
    {
        public string Status { get; set; }
        public HttpResponseException()
        {
        }

        public HttpResponseException(string message, string status)
            : base(message)
        {
            Status = status;
        }
    }
}
