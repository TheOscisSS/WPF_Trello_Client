using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Exceptions
{
    class ServerResponseException : Exception
    {
        public ServerResponseException(string message)
        : base(message)
        { }
    }
}
