﻿using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using WPF_Trello.Events;
using WPF_Trello.Messages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;
        private readonly MessageBusService _messageBusService;
        private readonly WebSocketService _webSocketService;

        private Models.Board _selectedBoard;

        public ObservableCollection<Models.Board> Boards { get; set; }
        public Models.Board SelectedBoard
        {
            get { return _selectedBoard; }
            set
            {
                _selectedBoard = value;
                if (_selectedBoard != null) 
                {
                    SendToBoardPreloadData();
                }
            }
        }

        public HomeViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                BoardService boardService, MessageBusService messageBusService, WebSocketService webSocketService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _messageBusService = messageBusService;
            _webSocketService = webSocketService;

            Boards = new ObservableCollection<Models.Board>();
            _selectedBoard = null;

            _eventBusService.Subscribe<AuthorizatedEvent>(async _ => Debug.WriteLine("Authorizated"));
            _eventBusService.Subscribe<CreatedUserEvent>(async _ => Debug.WriteLine("Create new account"));
            _eventBusService.Subscribe<GetBoardsEvent>(async _ => RenderBoards());
            _eventBusService.Subscribe<GoToHomeEvent>(async _ => 
            {
                _webSocketService.LeaveFromBoard();
                _selectedBoard = null;
            });
            _eventBusService.Subscribe<LogOutEvent>(async _ =>
            {
                Boards = new ObservableCollection<Models.Board>();
                _selectedBoard = null;
            });

            _messageBusService.Receive<CreateBoardMessage>(this, async message =>
            {
                AddNewBoard(message.Title, message.Background);
            });
            _messageBusService.Receive<AddNewBoardMessage>(this, async message =>
            {
                Boards.Add(message.Board);
            });
            _messageBusService.Receive<KickOutMemberMessage>(this, async message =>
            {
                var DeleteBoardById = Boards.FirstOrDefault(board => board.ID == message.BoardID) as Models.Board;
                Boards.Remove(DeleteBoardById);
            });
            _messageBusService.Receive<DeleteBoardMessage>(this, async message =>
            {
                var DeleteBoardById = Boards.FirstOrDefault(board => board.ID == message.BoarID) as Models.Board;
                Boards.Remove(DeleteBoardById);
            });
            
        }

        private async void SendToBoardPreloadData()
        {
            await _messageBusService.SendTo<BoardViewModel>(new BoardPreloadMessage(_selectedBoard.ID, _selectedBoard.Title,
                   _selectedBoard.Description, _selectedBoard.Background, _selectedBoard.CreatedAt, _selectedBoard.UpdatedAt));

            _webSocketService.JoinIntoBoard(_selectedBoard.ID);
            _pageService.ChangePage(new Pages.Board());
        }

        private async void AddNewBoard(string title, string background)
        {
            try
            {
                var newBoardItem = await _boardService.CreateNewBoard(title, background);
                Boards.Add(newBoardItem);

                await _messageBusService.SendTo<MainViewModel>(new NewBoardResponseMessage(true, string.Empty));
            }
            catch (UnauthorizedAccessException e)
            {
                //TODO: Add error handler
                Debug.WriteLine(e.Message);
                await _messageBusService.SendTo<MainViewModel>(new NewBoardResponseMessage(false, e.Message));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await _messageBusService.SendTo<MainViewModel>(new NewBoardResponseMessage(false, "Something was wrong"));
            }
        }

        private async void RenderBoards()
        {
            try
            {
                Boards = new ObservableCollection<Models.Board>(await _boardService.GetAllBoards());
            }
            catch (UnauthorizedAccessException e)
            {
                //TODO: Add error handler
                Debug.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
