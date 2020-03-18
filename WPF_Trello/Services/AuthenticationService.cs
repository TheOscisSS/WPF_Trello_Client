using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Services
{
    public class User
    {
        public User(string username, string[] roles)
        {
            Username = username;
            Roles = roles;
        }
        public string Username { get; set; }
        public string[] Roles { get; set; }
    }

    public class AuthenticationService
    {
        private class InternalUserData
        {
            public string Username { get; private set; }
            public string Password { get; private set; }
            public string[] Roles { get; private set; }

            public InternalUserData(string username, string password, string[] roles)
            {
                Username = username;
                Password = password;
                Roles = roles;
            }
        }

        private static readonly List<InternalUserData> _users = new List<InternalUserData>()
        {
            new InternalUserData("Dima", "123", new string[] { }),
        };

        public User AuthenticateUser(string username, string password)
        {
            InternalUserData userData = _users.FirstOrDefault(data => data.Username.Equals(username)
                && data.Password.Equals(password));

            if(userData == null)
            {
                throw new UnauthorizedAccessException("Please enter a valid data");
            }

            return new User(userData.Username, userData.Roles);
        }
    }
}
