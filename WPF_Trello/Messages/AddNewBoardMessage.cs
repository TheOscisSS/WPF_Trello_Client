using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class AddNewBoardMessage : IMessage
    {
        public AddNewBoardMessage(Board board)
        {
            Board = board;
        }

        public AddNewBoardMessage(Board board, string senderID) : this(board)
        {
            SenderID = senderID;
        }

        public Board Board { get; private set; }
        public string SenderID { get; private set; }
    }
}
