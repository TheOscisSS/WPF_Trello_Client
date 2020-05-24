using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class DeleteBoardMessage : IMessage
    {
        public DeleteBoardMessage(string boarID, string senderID)
        {
            BoarID = boarID;
            SenderID = senderID;
        }

        public string BoarID { get; private set; }
        public string SenderID { get; private set; }
    }
}
