namespace WPF_Trello.Messages
{
    class NewBoardResponseMessage : IMessage
    {
        public NewBoardResponseMessage(bool status, string responseMessage)
        {
            Status = status;
            ResponseMessage = responseMessage;
        }

        public bool Status { get; private set; }
        public string ResponseMessage { get; private set; }
    }
}
