using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class AddNewNotificationMessage : IMessage
    {
        public AddNewNotificationMessage(UserNotification userNotification)
        {
            UserNotification = userNotification;
        }

        public UserNotification UserNotification { get; private set; }
    }
}
