using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Trello.Models
{
    public class Board : BindableBase
    {
        public string Name { get; set; }
        public string Img { get; set; }
    }
}
