using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class AddNewActivityMessage : IMessage
    {
        public AddNewActivityMessage(BoardActivity addedActivity)
        {
            AddedActivity = addedActivity;
        }

        public BoardActivity AddedActivity { get; private set; }
    }
}
