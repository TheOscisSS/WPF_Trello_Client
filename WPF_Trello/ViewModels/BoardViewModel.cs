using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPF_Trello.Events;
using WPF_Trello.Messages;
using WPF_Trello.Models;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class BoardViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;
        private readonly MessageBusService _messageBusService;

        public string NewListTitle { get; set; }
        public Board CurrentBoard { get; private set; }
        //public ObservableCollection<BoardList> ListsSource { get; set; }
        public BoardList SelectedList { get; set; }
        public bool IsAddListTrigger { get; private set; }

        public BoardViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                BoardService boardService, MessageBusService messageBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _messageBusService = messageBusService;

            IsAddListTrigger = false;
            NewListTitle = string.Empty;

            _messageBusService.Receive<BoardPreloadMessage>(this, async message =>
            {
                CurrentBoard = new Board(message.ID, message.Title, message.Description, message.Background, message.CreatedAt, message.UpdatedAt);
                RenderBoard(message.ID);
            });
        }
        private async void RenderBoard(string id)
        {
            try
            {
                IsAddListTrigger = false;
                NewListTitle = string.Empty;
                CurrentBoard = await _boardService.GetBoardById(id);
                //ListsSource = CurrentBoard.Lists;
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
        public ICommand ShowAddListButton => new AsyncCommand(async () =>
        {
            IsAddListTrigger = true;
        });
        public ICommand HideAddListButton => new AsyncCommand(async () =>
        {
            IsAddListTrigger = false;
        });
        public ICommand ShowAddCardButton => new AsyncCommand(async () =>
        {
            Debug.WriteLine(SelectedList.Title);
            SelectedList.IsAddCardTrigger = true;
        });
        public ICommand HideAddCardButton => new AsyncCommand(async () =>
        {
            SelectedList.IsAddCardTrigger = false;
        });

        public ICommand AddAnotherListCommand => new AsyncCommand(async () =>
        {
            try
            {
                BoardList newList = await _boardService.CreateNewList(CurrentBoard.ID, NewListTitle);

                NewListTitle = string.Empty;
                //IsAddListTrigger = false;
                CurrentBoard.AddNewList(newList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        });

        public ICommand AddAnotherCardCommand => new AsyncCommand(async () =>
        {
            try
            {
                BoardCard newCard = await _boardService.CreateNewCard(SelectedList.ID, SelectedList.NewCardTitle);

                SelectedList.NewCardTitle = string.Empty;
                //SelectedList.IsAddCardTrigger = false;
                SelectedList.AddNewCard(newCard);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        });
    }
}
