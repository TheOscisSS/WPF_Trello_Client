using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    public class SendUserCredentialMessage : IMessage
    {
        public SendUserCredentialMessage(string message)
        {
            Message = message;
        }

        public SendUserCredentialMessage(User currentUser, string message)
        {
            CurrentUser = currentUser;
            Message = message;
        }

        public User CurrentUser { get; private set; }
        public string Message { get; private set; }
    }
}
