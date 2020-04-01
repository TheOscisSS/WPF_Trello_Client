using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    public class Board : BindableBase
    {
        public Board(
           string id,
           string title,
           string description,
           User owner,
           List<User> members,
           DateTime createdAt,
           DateTime updatedAt,
           List<BoardList> lists)
        {
            ID = id;
            Title = title;
            Description = description;
            Owner = owner;
            Members = members;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Lists = lists;
        }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User Owner { get; set; }
        public List<User> Members { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<BoardList> Lists { get; set; }
    }
}
