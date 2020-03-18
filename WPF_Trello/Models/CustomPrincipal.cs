using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    class CustomPrincipal : IPrincipal
    {
        CustomIdentity _customIdentity;

        CustomIdentity CustomIdentity {
            get => _customIdentity ?? new AnonymousIdentity();
            set { _customIdentity = value; } 
        }

        public IIdentity Identity => CustomIdentity;
        public bool IsInRole(string role) => 
            _customIdentity.Roles.Contains(role);
    }
}
