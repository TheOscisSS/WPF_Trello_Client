using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class DeleteBoardListMessage : IMessage
    {
        public DeleteBoardListMessage(string listID, string senderID)
        {
            ListID = listID;
            SenderID = senderID;
        }

        public string ListID { get; private set; }
        public string SenderID { get; private set; }
    }
}
