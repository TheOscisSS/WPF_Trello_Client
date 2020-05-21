namespace WPF_Trello.Messages
{
    class AddNewUserIconMessage : IMessage
    {
        public AddNewUserIconMessage(string icon)
        {
            Icon = icon;
        }

        public AddNewUserIconMessage(string icon, string senderID) : this(icon)
        {
            SenderID = senderID;
        }

        public string Icon { get; private set; }
        public string SenderID { get; private set; }
    }
}
