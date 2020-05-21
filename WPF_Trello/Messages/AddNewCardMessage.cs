using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class AddNewCardMessage : IMessage
    {
        public AddNewCardMessage(BoardCard boardCard, string listID, string senderID)
        {
            BoardCard = boardCard;
            ListID = listID;
            SenderID = senderID;
        }

        public BoardCard BoardCard { get; private set; }
        public string ListID { get; private set; }
        public string SenderID { get; private set; }
    }
}
