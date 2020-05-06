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
        public string InviteMemberName { get; set; }
        public Board CurrentBoard { get; private set; }
        public ObservableCollection<User> BoardMembers { get; private set; }
        public BoardList SelectedList { get; set; }
        public bool IsAddListTrigger { get; private set; }
        public bool IsMenuTrigger { get; private set; }
        public bool IsInviteMemberTrigger { get; private set; }

        public BoardViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                BoardService boardService, MessageBusService messageBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _messageBusService = messageBusService;

            IsAddListTrigger = false;
            IsMenuTrigger = false;
            IsInviteMemberTrigger = false;
            NewListTitle = string.Empty;
            InviteMemberName = string.Empty;

            _messageBusService.Receive<BoardPreloadMessage>(this, async message =>
            {
                CurrentBoard = new Board(message.ID, message.Title, message.Description, message.Background, message.CreatedAt, message.UpdatedAt);
                BoardMembers = new ObservableCollection<User>();
                RenderBoard(message.ID);
            });
        }
        private async void RenderBoard(string id)
        {
            try
            {
                IsAddListTrigger = false;
                IsInviteMemberTrigger = false;
                NewListTitle = string.Empty;
                InviteMemberName = string.Empty;
                CurrentBoard = await _boardService.GetBoardById(id);
                BoardMembers.Add(CurrentBoard.Owner);
                foreach(User member in CurrentBoard.Members)
                {
                    BoardMembers.Add(member);
                }
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
        public ICommand ShowMenu => new AsyncCommand(async () =>
        {
            IsMenuTrigger = true;
        });
        public ICommand HideMenu => new AsyncCommand(async () =>
        {
            IsMenuTrigger = false;
        });
        public ICommand ShowAddCardButton => new AsyncCommand(async () =>
        {
            SelectedList.IsAddCardTrigger = true;
        });
        public ICommand HideAddCardButton => new AsyncCommand(async () =>
        {
            SelectedList.IsAddCardTrigger = false;
        });
        public ICommand ShowInviteButton => new AsyncCommand(async () =>
        {
            IsInviteMemberTrigger = true;
        });
        public ICommand HideInviteButton => new AsyncCommand(async () =>
        {
            IsInviteMemberTrigger = false;
        });
        public ICommand AddAnotherListCommand => new AsyncCommand(async () =>
        {
            try
            {
                BoardList newList = await _boardService.CreateNewList(CurrentBoard.ID, NewListTitle);

                NewListTitle = string.Empty;
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
                SelectedList.AddNewCard(newCard);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand InviteMemberCommand => new AsyncCommand(async () =>
        {
            try
            {
                User newMember = await _boardService.InviteNewMember(CurrentBoard.ID, InviteMemberName);

                InviteMemberName = string.Empty;
                BoardMembers.Add(newMember);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        });
    }
}
