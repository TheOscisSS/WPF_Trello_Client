﻿using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class AddNewListMessage : IMessage
    {
        public AddNewListMessage(BoardList boardList)
        {
            BoardList = boardList;
        }

        public AddNewListMessage(BoardList boardList, string senderID) : this(boardList)
        {
            SenderID = senderID;
        }

        public BoardList BoardList { get; private set; }
        public string SenderID { get; private set; }
    }
}
