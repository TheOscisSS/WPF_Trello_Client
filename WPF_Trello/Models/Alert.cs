using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    public struct AlertStatus
    {
        public static string ERROR = "#eb5a46";
        public static string SUCCESS = "#5eba7d";
        public static string WARNING = "#FFCC00";
    }
    public class Alert
    {
        public string Message { get; private set; }
        public int Delay { get; private set; }
        public string Status { get; private set; }

        public Alert(string message, string status, int delay = 2500)
        {
            Message = message;
            Status = status;
            Delay = delay;
        }
    }
}
