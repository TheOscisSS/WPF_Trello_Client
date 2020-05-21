using System.Security.Principal;

namespace WPF_Trello.Models
{
    public class CustomIdentity : IIdentity
    {
        public string Name { get; private set; }
        public string[] Roles { get; private set; }

        public CustomIdentity(string name, string[] roles)
        {
            Name = name;
            Roles = roles;
        }

        public string AuthenticationType { get; }
        public bool IsAuthenticated { get => !string.IsNullOrEmpty(Name); }
    }
}
