﻿using DevExpress.Mvvm;
using GongSolutions.Wpf.DragDrop;
using Markdig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Trello.Events;
using WPF_Trello.Messages;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class BoardViewModel : BindableBase, IDropTarget
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;
        private readonly MessageBusService _messageBusService;
        private readonly WebSocketService _webSocketService;

        public string CardDetailID { get; private set; }
        public string CardDetailTitle { get; private set; }
        public string CardDetailCurrentList { get; private set; }
        public string CardDetailDescription { get; private set; }
        public string CardDetail { get; private set; }
        public string DescriptionTextBox { get; set; }
        public string CardDetailChangedBy { get; private set; }
        public string NewListTitle { get; set; }
        public string InviteMemberName { get; set; }
        public Models.Board CurrentBoard { get; private set; }
        public ObservableCollection<User> BoardMembers { get; set; }
        public BoardList SelectedList { get; set; }
        public BoardCard SelectedCard { get; set; }
        public User SelectedMember { get; set; }
        public bool IsAddListTrigger { get; private set; }
        public bool IsMenuTrigger { get; private set; }
        public bool IsInviteMemberTrigger { get; private set; }
        public bool IsShowCardDetails { get; private set; }
        public bool IsEditingDescription { get; private set; }
        public bool IsExistConflictChanges { get; private set; }


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
            SelectedCard = null;
            IsAddListTrigger = false;
            IsMenuTrigger = false;
            IsInviteMemberTrigger = false;
            IsShowCardDetails = false;
            IsEditingDescription = false;
            IsExistConflictChanges = false;
            NewListTitle = string.Empty;
            InviteMemberName = string.Empty;
            CardDetailID = string.Empty;
            CardDetailTitle = string.Empty;
            CardDetailCurrentList = string.Empty;
            CardDetailDescription = string.Empty;
            DescriptionTextBox = string.Empty;
            CardDetailChangedBy = string.Empty;

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
            _messageBusService.Receive<BoardDeleteMemberMessage>(this, async message =>
            {
                var deletedMemberById = BoardMembers.FirstOrDefault(member => member.ID == message.memberID);
                BoardMembers.Remove(deletedMemberById);
            });
            _messageBusService.Receive<KickOutMemberMessage>(this, async message =>
            {
                if (CurrentBoard.ID == message.BoardID)
                {
                    await _eventBusService.Publish(new GoToHomeEvent());
                    _pageService.ChangePage(new Home());
                }
            });
            _messageBusService.Receive<AddNewUserIconMessage>(this, async message =>
            {
                var memberById = BoardMembers.FirstOrDefault(member => member.ID == message.SenderID) as User;
                memberById.SetIcon(message.Icon);
                CurrentBoard.UpdateEachActivityIcon(message.Icon, message.SenderID);

            });
            _messageBusService.Receive<MoveBoardListMessage>(this, async message =>
            {
                var movableList = CurrentBoard.Lists.ElementAt(message.From);

                if(_authenticationService.CurrentUser.ID != message.SenderID)
                {
                    CurrentBoard.Lists.Remove(movableList);
                    CurrentBoard.Lists.Insert(message.To, movableList);
                }
            });
            _messageBusService.Receive<MoveBoardCardMessage>(this, async message =>
            {
                var collection = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ListID);
               
                if (_authenticationService.CurrentUser.ID != message.SenderID)
                {
                    var movableCard = collection.Cards.ElementAt(message.From);
                    var isEditingMovableCard = SelectedCard != null && SelectedCard.ID == movableCard.ID;

                    collection.Cards.Remove(movableCard);
                    collection.Cards.Insert(message.To, movableCard);

                    if (IsShowCardDetails && isEditingMovableCard)
                    {
                        SelectedCard = movableCard;
                        CardDetailCurrentList = collection.Title;
                        RaisePropertiesChanged("SelectedCard");
                        //RaisePropertiesChanged("SelectedCard.Title");
                    }
                }
            });
            _messageBusService.Receive<MoveCardBetweenListsMessage>(this, async message =>
            {
                var sourceCollection = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.FromListID);
                var targetCollection = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ToListID);

                if (_authenticationService.CurrentUser.ID != message.SenderID)
                {
                    var movableCard = sourceCollection.Cards.ElementAt(message.From);
                    var isEditingMovableCard = SelectedCard != null && SelectedCard.ID == movableCard.ID;

                    sourceCollection.Cards.Remove(movableCard);
                    targetCollection.Cards.Insert(message.To, movableCard);

                    if (IsShowCardDetails && isEditingMovableCard)
                    {
                        SelectedCard = movableCard;
                        CardDetailCurrentList = targetCollection.Title;
                        RaisePropertiesChanged("SelectedCard");
                        //RaisePropertiesChanged("SelectedCard.Title");
                    }
                }
            });
            _messageBusService.Receive<UpdateDescriptionMessage>(this, async message =>
            {
                if(IsShowCardDetails && SelectedCard.ID == message.CardID && 
                    _authenticationService.CurrentUser.ID != message.SenderID)
                {
                    SelectedCard.SetDescription(message.Description);
                    CardDetailDescription = Markdown.ToHtml(SelectedCard.Description);

                    if (IsEditingDescription)
                    {
                        var sender = BoardMembers.FirstOrDefault(user => user.ID == message.SenderID);
                        CardDetailChangedBy = sender.Username;
                        IsExistConflictChanges = true;
                    }
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

        public void DragOver(IDropInfo dropInfo)
        {
            if(dropInfo.Data is BoardList)
            {
                BoardList sourceItem = dropInfo.Data as BoardList;
                BoardList targetItem = dropInfo.TargetItem as BoardList;

                if (sourceItem != null && targetItem != null)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
            else if(dropInfo.Data is BoardCard && dropInfo.TargetItem is BoardCard)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;
                BoardCard targetItem = dropInfo.TargetItem as BoardCard;

                if (sourceItem != null && targetItem != null)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
            else if(dropInfo.TargetCollection is ObservableCollection<BoardCard>)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;
                var targetCollection = dropInfo.TargetCollection as ObservableCollection<BoardCard>;

                if (sourceItem != null && targetCollection != null && targetCollection.Count == 0)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
            
        }

        public void Drop(IDropInfo dropInfo)
        {

            if(dropInfo.Data is BoardList)
            {
                BoardList sourceItem = dropInfo.Data as BoardList;
                BoardList targetItem = dropInfo.TargetItem as BoardList;

                if (sourceItem.ID != targetItem.ID)
                {
                    int sourceIndex = CurrentBoard.Lists.IndexOf(sourceItem);
                    int targetIndex = CurrentBoard.Lists.IndexOf(targetItem);

                    CurrentBoard.Lists.Remove(sourceItem);
                    if (sourceIndex < targetIndex)
                    {
                        int toIndex = dropInfo.InsertIndex - 1 < 0 ? 0 : dropInfo.InsertIndex - 1;
                        SendMoveListData(sourceIndex, toIndex, CurrentBoard.ID);
                        CurrentBoard.Lists.Insert(toIndex, sourceItem);
                    }
                    else
                    {
                        SendMoveListData(sourceIndex, dropInfo.InsertIndex, CurrentBoard.ID);
                        CurrentBoard.Lists.Insert(dropInfo.InsertIndex, sourceItem);
                    }
                }
            }
            else if (dropInfo.Data is BoardCard && dropInfo.TargetItem is BoardCard)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;
                BoardCard targetItem = dropInfo.TargetItem as BoardCard;

                var sourceCollection = dropInfo.DragInfo.SourceCollection as ObservableCollection<BoardCard>;
                var targetCollection = dropInfo.TargetCollection as ObservableCollection<BoardCard>;

                if (sourceItem.ID != targetItem.ID)
                {
                    int sourceIndex = sourceCollection.IndexOf(sourceItem);
                    int targetIndex = targetCollection.IndexOf(targetItem);

                    var sourceListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == sourceCollection);
                    var currentListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == targetCollection);

                    sourceCollection.Remove(sourceItem);

                    if (sourceCollection == targetCollection)
                    {
                       
                        if (sourceIndex < targetIndex)
                        {
                            int toIndex = dropInfo.InsertIndex - 1 < 0 ? 0 : dropInfo.InsertIndex - 1;
                            SendMoveCardData(sourceIndex, toIndex, currentListCollection.ID);
                            targetCollection.Insert(toIndex, sourceItem);
                        }
                        else
                        {
                            SendMoveCardData(sourceIndex, dropInfo.InsertIndex, currentListCollection.ID);
                            targetCollection.Insert(dropInfo.InsertIndex, sourceItem);
                        }
                    }
                    else
                    {
                        SendMoveCardToAnotherListData(sourceIndex, dropInfo.InsertIndex, sourceListCollection.ID, currentListCollection.ID);

                        targetCollection.Insert(dropInfo.InsertIndex, sourceItem);
                    }
                    
                }

                SelectedCard = null;
            }
            else if (dropInfo.TargetCollection is ObservableCollection<BoardCard>)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;

                var sourceCollection = dropInfo.DragInfo.SourceCollection as ObservableCollection<BoardCard>;
                var targetCollection = dropInfo.TargetCollection as ObservableCollection<BoardCard>;

                var sourceListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == sourceCollection);
                var currentListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == targetCollection);

                if (targetCollection.Count == 0)
                {
                    int sourceIndex = sourceCollection.IndexOf(sourceItem);

                    SendMoveCardToAnotherListData(sourceIndex, 0, sourceListCollection.ID, currentListCollection.ID);

                    sourceCollection.Remove(sourceItem);
                    targetCollection.Add(sourceItem);
                }

                SelectedCard = null;
            }
        }
        private void UpdateListDetails()
        {

        }
        private async Task<List<string>> SendMoveListData(int from, int to, string boardID)
        {
            return await _boardService.MoveBoardList(from, to, boardID);
        }
        private async Task<List<string>> SendMoveCardData(int from, int to, string listID)
        {
            return await _boardService.MoveBoardCard(from, to, listID);
        }
        private async void SendMoveCardToAnotherListData(int from, int to, string fromListID, string toListID)
        {
            await _boardService.MoveBoardCardBetweenLists(from, to, fromListID, toListID);
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
        public ICommand StartEditingDescripntionCommand => new AsyncCommand(async () =>
        {
            DescriptionTextBox = Markdown.Normalize(SelectedCard.Description);
            IsEditingDescription = true;
        });
        public ICommand SaveEditingDescriptionCommand => new AsyncCommand(async () =>
        {
            var newCard = await _boardService.SetDescription(SelectedCard.ID, DescriptionTextBox);
            CardDetailDescription = Markdown.ToHtml(newCard.Description);
            SelectedCard.SetDescription(newCard.Description);

            IsEditingDescription = false;
            CardDetailChangedBy = string.Empty;
            IsExistConflictChanges = false;

        });
        public ICommand CancelEditingDescriptionCommand => new AsyncCommand(async () =>
        {
            IsEditingDescription = false;
            CardDetailChangedBy = string.Empty;
            IsExistConflictChanges = false;
        });
        public ICommand ShowCardDetailsCommand => new AsyncCommand(async () =>
        {
            CardDetailCurrentList = string.Copy(SelectedList.Title);
            IsEditingDescription = false;
            IsShowCardDetails = true;
            SelectedCard.SetDescription(await _boardService.GetDescription(SelectedCard.ID));
            CardDetailDescription = Markdown.ToHtml(SelectedCard.Description);
        });
        public ICommand HideCardDetailsCommand => new AsyncCommand(async () =>
        {

            if (IsEditingDescription)
            {
                if (!IsExistConflictChanges)
                {
                    var newCard = await _boardService.SetDescription(SelectedCard.ID, DescriptionTextBox);
                    CardDetailDescription = Markdown.ToHtml(newCard.Description);
                    SelectedCard.SetDescription(newCard.Description);
                }
                IsEditingDescription = false;
            }

            IsShowCardDetails = false;
            CardDetailCurrentList = string.Empty;
            CardDetailDescription = string.Empty;
            DescriptionTextBox = string.Empty;
            CardDetailChangedBy = string.Empty;
            IsExistConflictChanges = false;

            SelectedCard = null;
        });
        public ICommand DeleteMemberCommand => new AsyncCommand(async () =>
        {
            try
            {
                if(_authenticationService.CurrentUser.ID != SelectedMember.ID &&
                    CurrentBoard.Owner.ID != SelectedMember.ID)
                {
                    await _boardService.KickOutMemberFromBoardById(CurrentBoard.ID, string.Copy(SelectedMember.ID));
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
