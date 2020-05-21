using DevExpress.Mvvm;
using System;

namespace WPF_Trello.Models
{
    public class BoardCard : BindableBase
    {
        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public CardLabel Label { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public BoardCard(
            string id,
            string title, 
            string description,
            DateTime createdAt,
            DateTime updatedAt)
        {
            ID = id;
            Title = title;
            Description = description;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }
    }
}
