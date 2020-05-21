using System;

namespace WPF_Trello.Messages
{
    class BoardPreloadMessage : IMessage
    {
        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Background { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public BoardPreloadMessage(
            string id, 
            string title, 
            string description, 
            string background, 
            DateTime createdAt, 
            DateTime updatedAt)
        {
            ID = id;
            Title = title;
            Background = background;
            Description = description;
            createdAt = CreatedAt;
            updatedAt = UpdatedAt;
        }
    }
}
