using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class MoveBoardCardMessage : IMessage
    {
        public MoveBoardCardMessage(int from, int to, string senderID, string listID)
        {
            From = from;
            To = to;
            SenderID = senderID;
            ListID = listID;
        }

        public int From { get; private set; }
        public int To { get; private set; }
        public string SenderID { get; private set; }
        public string ListID { get; private set; }
    }
}
