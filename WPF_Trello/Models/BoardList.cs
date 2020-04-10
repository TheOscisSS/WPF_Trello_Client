using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    public class BoardList
    {
        public string Title { get; private set; }
        public List<BoardCard> Cards { get; private set; }

        public BoardList(string title, List<BoardCard> cards)
        {
            Title = title;
            Cards = cards;
        }
    }
}
