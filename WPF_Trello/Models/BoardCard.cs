using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    public class BoardCard
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Label { get; private set; }

        public BoardCard(string title, string description, string label)
        {
            Title = title;
            Description = description;
            Label = label;
        }
    }
}
