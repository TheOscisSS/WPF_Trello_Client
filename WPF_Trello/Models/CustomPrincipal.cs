using System;
using System.Linq;
using System.Security.Principal;

namespace WPF_Trello.Models
{
    public class CustomPrincipal : IPrincipal
    {
        private CustomIdentity _identity;

        public CustomIdentity Identity {
            get => _identity ?? new AnonymousIdentity();
            set { _identity = value; } 
        }

        IIdentity IPrincipal.Identity {
            get => Identity;
        }

        public bool IsInRole(string role) => 
            _identity.Roles.Contains(role);
    }
}
