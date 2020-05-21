using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace WPF_Trello.Models
{
    public class BoardList : BindableBase
    {
        public string ID { get; private set; }
        public string Title { get; private set; }
        public ObservableCollection<BoardCard> Cards { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public BoardList(
            string id,
            string title,
            ObservableCollection<BoardCard> cards,
            DateTime createdAt,
            DateTime updatedAt)
        {
            ID = id;
            Title = title;
            Cards = cards;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;

            IsAddCardTrigger = false;
            NewCardTitle = string.Empty;
        }

        public void AddNewCard(BoardCard newCard)
        {
            Cards.Add(newCard);
        }

        public bool IsAddCardTrigger { get; set; }
        public string NewCardTitle { get; set; }
    }
}
