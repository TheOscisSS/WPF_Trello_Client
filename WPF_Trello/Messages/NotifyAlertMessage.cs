using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;

namespace WPF_Trello.Messages
{
    class NotifyAlertMessage : IMessage
    {
        public Alert Alert { get; private set; }

        public NotifyAlertMessage(Alert alert)
        {
            Alert = alert;
        }
    }
}
