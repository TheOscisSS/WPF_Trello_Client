namespace WPF_Trello.Messages
{
    class MoveBoardListMessage : IMessage
    {
        public MoveBoardListMessage(int from, int to, string senderID)
        {
            From = from;
            To = to;
            SenderID = senderID;
        }

        public int From { get; private set; }
        public int To { get; private set; }
        public string SenderID { get; private set; }
    }
}
