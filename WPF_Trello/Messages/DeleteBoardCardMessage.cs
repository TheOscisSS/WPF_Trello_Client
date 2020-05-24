using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class DeleteBoardCardMessage : IMessage
    {
        public DeleteBoardCardMessage(string listID, string cardID, string senderID)
        {
            ListID = listID;
            CardID = cardID;
            SenderID = senderID;
        }

        public string ListID { get; private set; }
        public string CardID { get; private set; }
        public string SenderID { get; private set; }
    }
}
