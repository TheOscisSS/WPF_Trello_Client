using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    public class TextMessage : IMessage
    {
        public TextMessage(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }
}
