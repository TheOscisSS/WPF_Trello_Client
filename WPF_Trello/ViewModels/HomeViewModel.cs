using DevExpress.Mvvm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class HomeViewModel : BindableBase
    {

        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        public string EmailInputField { get; set; }
        public string PasswordInputField { get; set; }
        public string ConfirmPasswordInputField { get; set; }

        public ObservableCollection<Board> Boards { get; set; }

        public HomeViewModel(PageService pageService, AuthenticationService authenticationService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;

            Boards = new ObservableCollection<Board>
            {
                new Board {Name="ReactJS project. Dmitry Fesik", Img="https://imgcomfort.com/Userfiles/Upload/images/illustration-geiranger.jpg"},
                new Board {Name="Front-end Students Lab", Img="https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcSdloN9vDgDJgeKa1VcgSK38PPKuhUx5GhZH9wMtgXKr2rlNl00"},
            };
        }
    }
}
