using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string FormattedMessage
        {
            get => $"{Sender.Username} {Message}";
        }
    }
}
