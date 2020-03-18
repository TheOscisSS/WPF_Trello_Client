using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    class AnonymousIdentity : CustomIdentity
    {
        public AnonymousIdentity() : base(string.Empty, new string[] { }) { }
    }
}
