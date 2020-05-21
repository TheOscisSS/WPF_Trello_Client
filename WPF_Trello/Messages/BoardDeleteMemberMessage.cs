namespace WPF_Trello.Messages
{
    class BoardDeleteMemberMessage : IMessage
    {
        public BoardDeleteMemberMessage(string boardID)
        {
            BoardID = boardID;
        }

        public BoardDeleteMemberMessage(string boardID, string senderID) : this(boardID)
        {
            this.senderID = senderID;
        }

        public BoardDeleteMemberMessage(string boardID, string senderID, string memberID) : this(boardID, senderID)
        {
            this.memberID = memberID;
        }

        public string BoardID { get; private set; }
        public string senderID { get; private set; }
        public string memberID { get; private set; }
    }
}
