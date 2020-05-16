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

        public KickOutMemberMessage(string boardID, string senderID, string memberID) : this(boardID, senderID)
        {
            this.memberID = memberID;
        }

        public string BoardID { get; private set; }
        public string senderID { get; private set; }
        public string memberID { get; private set; }
    }
}
