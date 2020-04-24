using DevExpress.Mvvm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPF_Trello.Events;
using WPF_Trello.Messages;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;
using WPF_Trello.Utils;

namespace WPF_Trello.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;
        private readonly MessageBusService _messageBusService;

        private Models.Board _selectedBoard;

        public ObservableCollection<Models.Board> Boards { get; set; }
        public Models.Board SelectedBoard
        {
            get { return _selectedBoard; }
            set
            {
                _selectedBoard = value;
                SendToBoardPreloadData();
            }
        }

        public HomeViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                BoardService boardService, MessageBusService messageBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _messageBusService = messageBusService;

            _eventBusService.Subscribe<AuthorizatedEvent>(async _ => Debug.WriteLine("Authorizated"));
            _eventBusService.Subscribe<CreatedUserEvent>(async _ => Debug.WriteLine("Create new account"));
            _eventBusService.Subscribe<GetBoardsEvent>(async _ => RenderBoards());
            _eventBusService.Subscribe<GoToHomeEvent>(async _ => _selectedBoard = null);
        }

        private async void SendToBoardPreloadData()
        {
            await _messageBusService.SendTo<BoardViewModel>(new BoardPreloadMessage(_selectedBoard.ID, _selectedBoard.Title,
                   _selectedBoard.Description, _selectedBoard.Background, _selectedBoard.CreatedAt, _selectedBoard.UpdatedAt));
            _pageService.ChangePage(new Pages.Board());

            _pageService.ChangePage(new Pages.Board());
        }

        private async void RenderBoards()
        {
            try
            {
                Boards = new ObservableCollection<Models.Board>(await _boardService.GetAllBoards());
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine(e.Message);
                //ShowStatus = e.Message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
