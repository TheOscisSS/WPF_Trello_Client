﻿using Newtonsoft.Json.Linq;
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
        //private class InternalUserData
        //{
        //    public string Username { get; private set; }
        //    public string Password { get; private set; }
        //    public string[] Roles { get; private set; }

        //    public InternalUserData(string username, string password, string[] roles)
        //    {
        //        Username = username;
        //        Password = password;
        //        Roles = roles;
        //    }
        //}

        public async Task<User> GetCurrentUser()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", await AccessToken.Load());
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "user/me"))
                {
                    HttpResponseMessage response = await client.SendAsync(request);

                    using (HttpContent content = response.Content)
                    {
                        string result = await content.ReadAsStringAsync();
                        var joResponse = JObject.Parse(result);

                        if (!response.IsSuccessStatusCode)
                        {
                            var joMessage = (JValue)joResponse.SelectToken("message");
                            throw new UnauthorizedAccessException(joMessage.ToString());
                        }

                        return HttpHelper.ParseJsonToUserCredentials(joResponse.ToString());
                    }
                }
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
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "user/signin"))
                {
                    request.Content = new StringContent("{\"username\":\""+ username +"\",\"password\":\""+ password +"\"}",
                                                        Encoding.UTF8,
                                                        "application/json");
                    HttpResponseMessage response = await client.SendAsync(request);

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

                        return HttpHelper.ParseJsonToUserCredentials(joUser.ToString());
                    }
                }
            }
        }

        public event Action<bool> OnStatusChanged;



        //private static List<InternalUserData> _users = new List<InternalUserData>()
        //{
        //    new InternalUserData("Dima", "123", new string[] { }),
        //};

        //public User AuthenticateUser(string username, string password)
        //{
        //    InternalUserData userData = _users.FirstOrDefault(data => data.Username.Equals(username)
        //        && data.Password.Equals(password));

        //    if (userData == null)
        //    {
        //        throw new UnauthorizedAccessException("Please enter a valid data");
        //    }

        //    return new User(userData.Username, userData.Roles);
        //}

        //public User CreateNewUser(string username, string password)
        //{
        //    InternalUserData userData = _users.FirstOrDefault(data => data.Username.Equals(username));

        //    if (userData != null)
        //    {
        //        throw new RegistrationException("Already exist user with the same name");
        //    }

        //    _users.Add(new InternalUserData(username, password, new string[] { }));

        //    return new User(username, new string[] { });
        //}
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
