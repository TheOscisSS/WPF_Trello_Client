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
using WPF_Trello.Pages;
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
        private readonly WebSocketService _webSocketService;

        public string NewListTitle { get; set; }
        public string InviteMemberName { get; set; }
        public Models.Board CurrentBoard { get; private set; }
        public ObservableCollection<User> BoardMembers { get; set; }
        public BoardList SelectedList { get; set; }
        public User SelectedMember { get; set; }
        public bool IsAddListTrigger { get; private set; }
        public bool IsMenuTrigger { get; private set; }
        public bool IsInviteMemberTrigger { get; private set; }


        public BoardViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                BoardService boardService, MessageBusService messageBusService, WebSocketService webSocketService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _messageBusService = messageBusService;
            _webSocketService = webSocketService;

            SelectedMember = null;
            IsAddListTrigger = false;
            IsMenuTrigger = false;
            IsInviteMemberTrigger = false;
            NewListTitle = string.Empty;
            InviteMemberName = string.Empty;

            _messageBusService.Receive<BoardPreloadMessage>(this, async message =>
            {
                CurrentBoard = new Models.Board(message.ID, message.Title, message.Description, message.Background, message.CreatedAt, message.UpdatedAt);
                BoardMembers = new ObservableCollection<User>();
                RenderBoard(message.ID);
            });
            _messageBusService.Receive<AddNewActivityMessage>(this, async message =>
            {
                CurrentBoard.AddNewActivity(message.AddedActivity);   
            });
            _messageBusService.Receive<AddNewMemberMessage>(this, async message =>
            {
                BoardMembers.Add(message.InvitedMember);
            });
            _messageBusService.Receive<AddNewListMessage>(this, async message =>
            {
                NewListTitle = string.Empty;
                CurrentBoard.AddNewList(message.BoardList);
            });
            _messageBusService.Receive<AddNewCardMessage>(this, async message =>
            {
                if(_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    SelectedList.AddNewCard(message.BoardCard);
                    SelectedList.NewCardTitle = string.Empty;
                }
                else
                {
                    var ListByID = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ListID);
                    ListByID.AddNewCard(message.BoardCard);
                }
            });
            _messageBusService.Receive<KickOutMemberMessage>(this, async message =>
            {
                if(CurrentBoard.ID == message.BoardID)
                {
                    await _eventBusService.Publish(new GoToHomeEvent());
                    _pageService.ChangePage(new Home());
                }
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
        public ICommand DeleteMemberCommand => new AsyncCommand(async () =>
        {
            try
            {
                if(_authenticationService.CurrentUser.ID != SelectedMember.ID)
                {
                    _boardService.KickOutMemberFromBoardById(CurrentBoard.ID, string.Copy(SelectedMember.ID));
                    BoardMembers.Remove(SelectedMember);
                }
                else
                {
                    //TODO:add error handler
                }
            }
            catch (Exception ex)
            {
                //TODO: add error handler
                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand AddAnotherListCommand => new AsyncCommand(async () =>
        {
            try
            {
                BoardList newList = await _boardService.CreateNewList(CurrentBoard.ID, NewListTitle);
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
                _webSocketService.NotifyInvitedMember(newMember.ID);

                InviteMemberName = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        });
    }
}
