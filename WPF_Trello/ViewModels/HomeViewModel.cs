using DevExpress.Mvvm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPF_Trello.Events;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class HomeViewModel : BindableBase
    {

        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;

        public ObservableCollection<Board> Boards { get; set; }

        public HomeViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;

            _eventBusService.Subscribe<AuthorizatedEvent>(async _ => Debug.WriteLine("Authorizated"));

            //Boards = new ObservableCollection<Board>
            //{
            //    new Board {Name="ReactJS project. Dmitry Fesik", Img="https://imgcomfort.com/Userfiles/Upload/images/illustration-geiranger.jpg"},
            //    new Board {Name="Front-end Students Lab", Img="https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcSdloN9vDgDJgeKa1VcgSK38PPKuhUx5GhZH9wMtgXKr2rlNl00"},
            //};
        }
    }
}
