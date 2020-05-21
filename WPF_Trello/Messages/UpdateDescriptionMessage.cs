using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class UpdateDescriptionMessage : IMessage
    {
        public UpdateDescriptionMessage(string senderID, string cardID, string description)
        {
            SenderID = senderID;
            CardID = cardID;
            Description = description;
        }

        public string SenderID { get; private set; }
        public string CardID { get; private set; }
        public string Description { get; private set; }
    }
}
