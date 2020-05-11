using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class KickOutMemberMessage : IMessage
    {
        public KickOutMemberMessage(string boardID)
        {
            BoardID = boardID;
        }

        public KickOutMemberMessage(string boardID, string senderID) : this(boardID)
        {
            this.senderID = senderID;
        }

        public string BoardID { get; private set; }
        public string senderID { get; private set; }
    }
}
