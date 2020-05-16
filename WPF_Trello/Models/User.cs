using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    public class User
    {

        public User(string username)
        {
            Username = username;
        }
        public User(
            string id,
            string username,
            string icon,
            DateTime createdAt,
            DateTime updatedAt,
            string[] roles)
        {
            ID = id;
            Username = username;
            Icon = icon;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public char FirstNameCharacter
        {
            get => Username[0];
        }
        public void SetIcon(string icon)
        {
            Icon = icon;
        }

        public string ID { get; private set; }
        public string Username { get; private set; }
        public string Icon { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
