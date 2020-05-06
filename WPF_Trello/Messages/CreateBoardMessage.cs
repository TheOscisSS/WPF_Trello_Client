using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Messages
{
    class CreateBoardMessage : IMessage
    {
        public CreateBoardMessage(string title, string background)
        {
            Title = title;
            Background = background;
        }

        public string Title { get; private set; }
        public string Background { get; private set; }
    }
}
