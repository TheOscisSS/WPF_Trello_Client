using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class MoveCardBetweenListsMessage : IMessage
    {
        public MoveCardBetweenListsMessage(string fromListID, string toListID, int from, int to, string senderID)
        {
            FromListID = fromListID;
            ToListID = toListID;
            From = from;
            To = to;
            SenderID = senderID;
        }

        public string FromListID { get; private set; }
        public string ToListID { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }
        public string SenderID { get; private set; }
    }
}
