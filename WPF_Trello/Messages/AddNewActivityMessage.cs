using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class AddNewActivityMessage : IMessage
    {
        public AddNewActivityMessage(BoardActivity addedActivity)
        {
            AddedActivity = addedActivity;
        }

        public BoardActivity AddedActivity { get; private set; }
    }
}
