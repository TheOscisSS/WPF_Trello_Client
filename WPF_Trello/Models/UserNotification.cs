using DevExpress.Mvvm;
using System;

namespace WPF_Trello.Models
{
    public class UserNotification : BindableBase
    {
        public UserNotification(
            string id,
            string message, 
            bool isReaded, 
            DateTime createdAt, 
            DateTime updatedAt)
        {
            ID = id;
            Message = message;
            IsReaded = isReaded;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
        public string ID { get; private set; }
        public string Message { get; private set; }
        public bool IsReaded { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public string IsReadedToString
        {
            get
            {
                if(IsReaded)
                {
                    return "Read";
                }
                else
                {
                    return "Unread";
                }
            }
        }

        public void ReadNotification()
        {
            IsReaded = true;
        }
    }
}
