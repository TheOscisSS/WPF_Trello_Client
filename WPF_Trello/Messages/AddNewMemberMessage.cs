﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class AddNewMemberMessage : IMessage
    {
        public AddNewMemberMessage(User invitedMember)
        {
            InvitedMember = invitedMember;
        }

        public AddNewMemberMessage(User invitedMember, string senderID) : this(invitedMember)
        {
            SenderID = senderID;
        }

        public User InvitedMember { get; private set; }
        public string SenderID { get; private set; }
    }
}
