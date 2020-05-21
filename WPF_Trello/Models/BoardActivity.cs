using DevExpress.Mvvm;
using System;

namespace WPF_Trello.Models
{
    public class BoardActivity : BindableBase
    {
        public BoardActivity(
            string id, 
            string message, 
            User sender, 
            DateTime createdAt, 
            DateTime updatedAt)
        {
            ID = id;
            Message = message;
            Sender = sender;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public string ID { get; private set; }
        public string Message { get; private set; }
        public User Sender { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public void SetIcon(string icon)
        {
            Sender.SetIcon(icon);
            RaisePropertiesChanged("Sender");
        }

        public string FormattedMessage
        {
            get => $"{Sender.Username} {Message}";
        }
        public string CreatedAtToString
        {
            get => CreatedAt.ToUniversalTime().ToString();
        }
    }
}
