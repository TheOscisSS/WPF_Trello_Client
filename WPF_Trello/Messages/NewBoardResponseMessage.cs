using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class NewBoardResponseMessage : IMessage
    {
        public NewBoardResponseMessage(bool status, string responseMessage)
        {
            Status = status;
            ResponseMessage = responseMessage;
        }

        public bool Status { get; private set; }
        public string ResponseMessage { get; private set; }
    }
}
