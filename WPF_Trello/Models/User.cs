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
            DateTime createdAt,
            DateTime updatedAt,
            string[] roles)
        {
            ID = id;
            Username = username;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
        public string ID { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
